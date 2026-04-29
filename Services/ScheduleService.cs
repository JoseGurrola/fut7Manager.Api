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

                //rotación
                var last = workingTeams[^1];
                workingTeams.RemoveAt(workingTeams.Count - 1);
                workingTeams.Insert(1, last);
            }

            return matchdays;
        }

        public async Task<LeagueDashboardDto> GetDashboardAsync(int leagueId) {
            // =========================
            // 🔹 1. Jornada actual
            // =========================
            var currentMatchday = await _context.Matchdays
                .Include(md => md.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(md => md.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .Where(md => md.LeagueId == leagueId)
                .OrderBy(md => md.Number)
                .FirstOrDefaultAsync(md =>
                    md.Matches.Any(m => m.HomeGoals == null || m.AwayGoals == null)
                );

            MatchdayDto? matchdayDto = null;

            if (currentMatchday != null) {
                var allTeams = await _context.Teams
                    .Where(t => t.LeagueId == leagueId)
                    .ToListAsync();

                currentMatchday.RestingTeamNames = allTeams
                    .Where(t => !currentMatchday.Matches.Any(m =>
                        m.HomeTeamId == t.Id || m.AwayTeamId == t.Id))
                    .Select(t => t.Name)
                    .ToList();

                matchdayDto = _mapper.Map<MatchdayDto>(currentMatchday);
            }

            // =========================
            // 🔹 2. Equipos (con grupo)
            // =========================
            var teams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();

            // =========================
            // 🔹 3. Partidos jugados
            // =========================
            var matches = await _context.Matches
                .Where(m => m.LeagueId == leagueId &&
                            m.HomeGoals != null &&
                            m.AwayGoals != null)
                .ToListAsync();

            // =========================
            // 🔹 4. Agrupar por grupo
            // =========================
            var groupedStandings = new List<GroupStandingDto>();

            var teamsByGroup = teams
                .GroupBy(t => t.GroupId)
                .ToList();

            foreach (var group in teamsByGroup) {
                var groupId = group.Key;

                // 🔹 Inicializar standings del grupo
                var standingsDict = group.ToDictionary(
                    t => t.Id,
                    t => new StandingDto {
                        TeamId = t.Id,
                        TeamName = t.Name,
                        Played = 0,
                        Won = 0,
                        Draw = 0,
                        Lost = 0,
                        GoalsFor = 0,
                        GoalsAgainst = 0,
                        Points = 0
                    });

                // 🔹 Filtrar partidos del grupo
                var groupMatches = matches
                    .Where(m => m.GroupId == groupId)
                    .ToList();

                // 🔹 Procesar partidos
                foreach (var m in groupMatches) {
                    if (!standingsDict.ContainsKey(m.HomeTeamId) || !standingsDict.ContainsKey(m.AwayTeamId))
                        continue;

                    var home = standingsDict[m.HomeTeamId];
                    var away = standingsDict[m.AwayTeamId];

                    home.Played++;
                    away.Played++;

                    if (!m.HomeGoals.HasValue || !m.AwayGoals.HasValue)
                        continue;

                    home.GoalsFor += m.HomeGoals.Value;
                    home.GoalsAgainst += m.AwayGoals.Value;

                    away.GoalsFor += m.AwayGoals.Value;
                    away.GoalsAgainst += m.HomeGoals.Value;

                    if (m.HomeGoals > m.AwayGoals) {
                        home.Won++; home.Points += 3;
                        away.Lost++;
                    } else if (m.HomeGoals < m.AwayGoals) {
                        away.Won++; away.Points += 3;
                        home.Lost++;
                    } else {
                        home.Draw++; away.Draw++;
                        home.Points++; away.Points++;
                    }
                }

                // 🔹 Ordenar
                var standings = standingsDict.Values
                    .OrderByDescending(t => t.Points)
                    .ThenByDescending(t => t.GoalDifference)
                    .ThenByDescending(t => t.GoalsFor)
                    .ThenBy(t => t.TeamName)
                    .ToList();

                // 🔹 Posiciones
                for (int i = 0; i < standings.Count; i++) {
                    standings[i].Position = i + 1;
                }

                var groups = await _context.Groups
                    .Where(g => g.LeagueId == leagueId)
                    .ToListAsync();

                groupedStandings.Add(new GroupStandingDto {
                    GroupName = groups.FirstOrDefault(g => g.Id == groupId)?.Name ?? $"Grupo {groupId}",
                    Standings = standings
                });
            }

            // =========================
            // 🔹 5. (Opcional) global fallback
            // =========================
            var flatStandings = groupedStandings
                .SelectMany(g => g.Standings)
                .OrderBy(s => s.Position)
                .ToList();

            // =========================
            // 🔹 6. Resultado
            // =========================
            return new LeagueDashboardDto {
                CurrentMatchday = matchdayDto,
                GroupedStandings = groupedStandings,
                Standings = flatStandings // fallback
            };
        }

        public async Task<StandingsResponseDto> GetStandingsAsync(int leagueId) {
            // =========================
            // 🔹 1. Equipos
            // =========================
            var teams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();

            // =========================
            // 🔹 2. Partidos jugados
            // =========================
            var matches = await _context.Matches
                .Where(m => m.LeagueId == leagueId &&
                            m.HomeGoals != null &&
                            m.AwayGoals != null)
                .OrderByDescending(m => m.MatchDate.HasValue)
                .ThenByDescending(m => m.MatchDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();

            // =========================
            // 🔹 3. Grupos
            // =========================
            var groups = await _context.Groups
                .Where(g => g.LeagueId == leagueId)
                .ToListAsync();

            var groupedStandings = new List<GroupStandingDto>();

            var teamsByGroup = teams.GroupBy(t => t.GroupId);

            foreach (var group in teamsByGroup) {
                var groupId = group.Key;

                var standingsDict = group.ToDictionary(
                    t => t.Id,
                    t => new StandingDto {
                        TeamId = t.Id,
                        TeamName = t.Name,
                        Played = 0,
                        Won = 0,
                        Draw = 0,
                        Lost = 0,
                        GoalsFor = 0,
                        GoalsAgainst = 0,
                        Points = 0,
                        Last5Results = new List<string>()
                    });

                var groupMatches = matches
                    .Where(m => m.GroupId == groupId)
                    .ToList();

                foreach (var m in groupMatches) {
                    var home = standingsDict[m.HomeTeamId];
                    var away = standingsDict[m.AwayTeamId];

                    home.Played++;
                    away.Played++;

                    home.GoalsFor += m.HomeGoals.Value;
                    home.GoalsAgainst += m.AwayGoals.Value;

                    away.GoalsFor += m.AwayGoals.Value;
                    away.GoalsAgainst += m.HomeGoals.Value;

                    // 🔹 Resultado
                    if (m.HomeGoals > m.AwayGoals) {
                        home.Won++; home.Points += 3;
                        away.Lost++;

                        AddResult(home, "W");
                        AddResult(away, "L");
                    } else if (m.HomeGoals < m.AwayGoals) {
                        away.Won++; away.Points += 3;
                        home.Lost++;

                        AddResult(home, "L");
                        AddResult(away, "W");
                    } else {
                        home.Draw++; away.Draw++;
                        home.Points++; away.Points++;

                        AddResult(home, "D");
                        AddResult(away, "D");
                    }
                }

                var standings = standingsDict.Values
                    .OrderByDescending(t => t.Points)
                    .ThenByDescending(t => t.GoalDifference)
                    .ThenByDescending(t => t.GoalsFor)
                    .ThenBy(t => t.TeamName)
                    .ToList();

                for (int i = 0; i < standings.Count; i++)
                    standings[i].Position = i + 1;

                groupedStandings.Add(new GroupStandingDto {
                    GroupName = groups.FirstOrDefault(g => g.Id == groupId)?.Name ?? $"Grupo {groupId}",
                    Standings = standings
                });
            }

            // =========================
            // 🔹 TABLA GENERAL
            // =========================
            var generalStandings = groupedStandings
                .SelectMany(g => g.Standings)
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.TeamName)
                .ToList();

            for (int i = 0; i < generalStandings.Count; i++)
                generalStandings[i].Position = i + 1;

            return new StandingsResponseDto {
                GroupedStandings = groupedStandings,
                Standings = generalStandings
            };
        }
        private void AddResult(StandingDto team, string result) {
            team.Last5Results.Insert(0, result); // más reciente primero

            if (team.Last5Results.Count > 5)
                team.Last5Results.RemoveAt(team.Last5Results.Count - 1);
        }
    }

    }