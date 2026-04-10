using AutoMapper;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services.Interfaces;
using fut7Manager.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace fut7Manager.Api.Services {
    public class ScheduleService : IScheduleService {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private static readonly Random _random = new Random();
        public ScheduleService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<MatchdayDto>> GenerateScheduleAsync(int leagueId, bool interGroupMatches) {
            var league = await _context.Leagues
                .Include(l => l.Groups)
                    .ThenInclude(g => g.Teams)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
                throw new Exception("League not found");

            if (league.Status != Helpers.LeagueStatus.Upcoming)
                throw new Exception("Schedule only accepted in Upcoming status");

            List<Matchday> matchdays;

            if (interGroupMatches)
                matchdays = GenerateInterGroup(league);
            else
                matchdays = GenerateIntraGroup(league);

            league.IsScheduleGenerated = true;
            league.InterGroupMatches = interGroupMatches;

            // 🔥 1. Obtener matchdays existentes
            var existingMatchdays = await _context.Matchdays
                .Where(m => m.LeagueId == leagueId)
                .ToListAsync();

            if (existingMatchdays.Any()) {
                // 🔥 2. Obtener matches relacionados
                var matchdayIds = existingMatchdays.Select(m => m.Id).ToList();

                var existingMatches = await _context.Matches
                    .Where(m => m.MatchdayId.HasValue && matchdayIds.Contains(m.MatchdayId.Value))
                    .ToListAsync();

                // 🔥 3. Borrar primero hijos
                _context.Matches.RemoveRange(existingMatches);

                // 🔥 4. Luego padres
                _context.Matchdays.RemoveRange(existingMatchdays);
            }

            // 🔥 5. Agregar nuevo calendario
            await _context.Matchdays.AddRangeAsync(matchdays);

            await _context.SaveChangesAsync();

            return _mapper.Map<List<MatchdayDto>>(matchdays);
        }

        // =========================
        // INTRA GROUP
        // =========================
        private List<Matchday> GenerateIntraGroup(League league) {
            var groupSchedules = new List<List<Matchday>>();

            // 🔹 1. Generar calendario por grupo
            foreach (var group in league.Groups) {
                var schedule = RoundRobin(
                    group.Teams.ToList(),
                    league.Id,
                    group.Id,
                    1 // temporal
                );

                groupSchedules.Add(schedule);
            }

            var result = new List<Matchday>();

            int maxMatchdays = groupSchedules.Max(g => g.Count);

            int globalMatchdayNumber = 1;

            // 🔹 2. Intercalar jornadas
            for (int i = 0; i < maxMatchdays; i++) {
                var matchday = new Matchday {
                    LeagueId = league.Id,
                    Number = globalMatchdayNumber++,
                    Matches = new List<Fut7Match>()
                };

                foreach (var groupSchedule in groupSchedules) {
                    if (i < groupSchedule.Count) {

                        var sourceMatchday = groupSchedule[i];

                        foreach (var match in sourceMatchday.Matches) {
                            matchday.Matches.Add(match);
                        }

                        matchday.RestingTeamNames.AddRange(sourceMatchday.RestingTeamNames);
                    }
                }

                result.Add(matchday);
            }

            return result;
        }

        // =========================
        // INTER GROUP
        // =========================
        private List<Matchday> GenerateInterGroup(League league) {
            var allTeams = league.Groups
                .SelectMany(g => g.Teams)
                .ToList();

            return RoundRobin(allTeams, league.Id, null, 1);
        }

        // =========================
        // ROUND ROBIN CORE
        // =========================
        private List<Matchday> RoundRobin(
    List<Team> teams,
    int leagueId,
    int? groupId,
    int startMatchdayNumber) {

            var matchdays = new List<Matchday>();

            var random = new Random();

            var workingTeams = teams
                 .OrderBy(x => _random.Next())
                 .ToList();

            // Si es impar → agregar BYE
            if (workingTeams.Count % 2 != 0)
                workingTeams.Add(new Team { Id = -1, Name = "BYE" });

            int numMatchdays = workingTeams.Count - 1;
            int matchesPerDay = workingTeams.Count / 2;

            for (int day = 0; day < numMatchdays; day++) {

                var matchday = new Matchday {
                    LeagueId = leagueId,
                    Number = startMatchdayNumber + day,
                    Matches = new List<Fut7Match>(),
                    RestingTeamNames = new List<string>()
                };

                for (int match = 0; match < matchesPerDay; match++) {

                    var home = workingTeams[match];
                    var away = workingTeams[workingTeams.Count - 1 - match];

                    // 👉 detectar descanso
                    if (home.Id == -1 && away.Id != -1) {
                        matchday.RestingTeamNames.Add(away.Name ?? $"Equipo {away.Id}");
                        continue;
                    }

                    if (away.Id == -1 && home.Id != -1) {
                        matchday.RestingTeamNames.Add(home.Name ?? $"Equipo {home.Id}");
                        continue;
                    }

                    if (home.Id == -1 || away.Id == -1)
                        continue;

                    matchday.Matches.Add(new Fut7Match {
                        LeagueId = leagueId,
                        GroupId = groupId ?? home.GroupId,
                        HomeTeamId = home.Id,
                        AwayTeamId = away.Id
                    });
                }

                matchdays.Add(matchday);

                // 🔄 rotación
                var last = workingTeams[^1];
                workingTeams.RemoveAt(workingTeams.Count - 1);
                workingTeams.Insert(1, last);
            }

            return matchdays;
        }
    }
}