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

            if (league.IsScheduleGenerated)
                throw new Exception("Schedule already generated");

            List<Matchday> matchdays;

            if (interGroupMatches)
                matchdays = GenerateInterGroup(league);
            else
                matchdays = GenerateIntraGroup(league);

            league.IsScheduleGenerated = true;
            league.InterGroupMatches = interGroupMatches;

            _context.Matchdays.AddRange(matchdays);
            await _context.SaveChangesAsync();

            return _mapper.Map<List<MatchdayDto>>(matchdays);
        }

        // =========================
        // INTRA GROUP
        // =========================
        private List<Matchday> GenerateIntraGroup(League league) {
            var allMatchdays = new List<Matchday>();
            int currentMatchdayNumber = 1;

            foreach (var group in league.Groups) {
                var groupMatchdays = RoundRobin(
                    group.Teams.ToList(),
                    league.Id,
                    group.Id,
                    currentMatchdayNumber);

                currentMatchdayNumber += groupMatchdays.Count;
                allMatchdays.AddRange(groupMatchdays);
            }

            return allMatchdays;
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

            // Si es impar, agregar BYE
            if (teams.Count % 2 != 0)
                teams.Add(new Team { Id = -1 });

            int numMatchdays = teams.Count - 1;
            int matchesPerDay = teams.Count / 2;

            for (int day = 0; day < numMatchdays; day++) {
                var matchday = new Matchday {
                    LeagueId = leagueId,
                    Number = startMatchdayNumber + day,
                    Matches = new List<Fut7Match>()
                };

                for (int match = 0; match < matchesPerDay; match++) {
                    var home = teams[match];
                    var away = teams[teams.Count - 1 - match];

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

                // rotación de equipos
                var last = teams[^1];
                teams.RemoveAt(teams.Count - 1);
                teams.Insert(1, last);
            }

            return matchdays;
        }
    }
}