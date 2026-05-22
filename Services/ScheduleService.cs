using AutoMapper;
using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services.Interfaces;
using fut7Manager.Data;
using Microsoft.EntityFrameworkCore;

namespace fut7Manager.Api.Services {
    public class ScheduleService : IScheduleService {

        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        private static readonly Random _random = new Random();

        public ScheduleService(
            ApplicationDbContext context,
            IMapper mapper) {

            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MatchdayDto>> PreviewScheduleAsync(
            int leagueId,
            GenerateScheduleDto dto) {
            var league = await _context.Leagues
                .AsNoTracking()
                .Include(l => l.Groups)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
                throw new Exception("League not found");

            if (league.Status != Helpers.LeagueStatus.Upcoming)
                throw new Exception("Schedule only accepted in Upcoming status");

            var allTeams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();

            // aplicar asignaciones temporales
            foreach (var a in dto.Teams) {
                var team = allTeams.FirstOrDefault(t => t.Id == a.TeamId);
                if (team != null)
                    team.GroupId = a.GroupId;
            }

            var groups = allTeams
                .GroupBy(t => t.GroupId)
                .Where(g => g.Key != null)
                .ToList();

            var fakeLeague = new League {
                Id = leagueId,
                Groups = groups
                    .Where(g => g.Key != null)
                    .Select(g => new Group {
                        Id = g.Key!.Value,
                        Teams = g.ToList()
                    })
                    .ToList()
            };

            var matchdays = dto.InterGroupMatches
                ? GenerateInterGroup(fakeLeague)
                : GenerateIntraGroup(fakeLeague);

            return matchdays.Select(md => new MatchdayDto {
                Number = md.Number,
                RestingTeamNames = md.RestingTeamNames,
                Matches = md.Matches.Select(m => {
                    var home = allTeams.First(t => t.Id == m.HomeTeamId);
                    var away = allTeams.First(t => t.Id == m.AwayTeamId);

                    return new Fut7MatchDto {
                        HomeTeamId = m.HomeTeamId,
                        AwayTeamId = m.AwayTeamId,
                        GroupId = m.GroupId,
                        HomeTeamName = home.Name,
                        AwayTeamName = away.Name,
                        HomeTeamLogo = home.LogoUrl,
                        AwayTeamLogo = away.LogoUrl
                    };
                }).ToList()
            }).ToList();
        }

        public async Task FinalizeSetupAsync(
            int leagueId,
            FinalizeLeagueSetupDto dto) {
            var league = await _context.Leagues
                .Include(l => l.Groups)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null)
                throw new Exception("League not found");

            var teams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();

            // asignar grupos
            foreach (var a in dto.Teams) {
                var team = teams.FirstOrDefault(t => t.Id == a.TeamId);
                if (team != null)
                    team.GroupId = a.GroupId;
            }

            await _context.SaveChangesAsync();

            // reconstruir grupos válidos (NO null)
            var groups = teams
                .Where(t => t.GroupId != null)
                .GroupBy(t => t.GroupId!.Value)
                .ToList();

            var leagueMemory = new League {
                Id = leagueId,
                Groups = groups.Select(g => new Group {
                    Id = g.Key,
                    Teams = g.ToList()
                }).ToList()
            };

            var matchdays = dto.InterGroupMatches
                ? GenerateInterGroup(leagueMemory)
                : GenerateIntraGroup(leagueMemory);

            // limpiar calendario anterior
            var existing = await _context.Matchdays
                .Where(m => m.LeagueId == leagueId)
                .ToListAsync();

            _context.Matchdays.RemoveRange(existing);
            await _context.SaveChangesAsync();

            // guardar nuevo calendario (FIX FK + NULL SAFE)
            var dbMatchdays = matchdays.Select(md => new Matchday {
                LeagueId = leagueId,
                Number = md.Number,
                RestingTeamNames = md.RestingTeamNames,

                Matches = md.Matches.Select(m => {
                    var home = teams.First(t => t.Id == m.HomeTeamId);

                    if (home.GroupId == null)
                        throw new Exception($"Team {home.Id} sin grupo asignado");

                    return new Fut7Match {
                        LeagueId = leagueId,
                        GroupId = home.GroupId.Value,
                        HomeTeamId = m.HomeTeamId,
                        AwayTeamId = m.AwayTeamId
                    };
                }).ToList()
            }).ToList();

            _context.Matchdays.AddRange(dbMatchdays);

            league.IsScheduleGenerated = true;
            league.InterGroupMatches = dto.InterGroupMatches;

            await _context.SaveChangesAsync();
        }

        // =========================
        // 🔹 INTRA GROUP
        // =========================

        private List<Matchday> GenerateIntraGroup(League league) {

            var groupSchedules = new List<List<Matchday>>();

            // 🔹 Generar calendario por grupo
            foreach (var group in league.Groups) {

                var schedule = RoundRobin(
                    group.Teams.ToList(),
                    league.Id,
                    group.Id,
                    1);

                groupSchedules.Add(schedule);
            }

            var result = new List<Matchday>();

            int maxMatchdays = groupSchedules.Max(g => g.Count);

            int globalMatchdayNumber = 1;

            // 🔹 Intercalar jornadas
            for (int i = 0; i < maxMatchdays; i++) {

                var matchday = new Matchday {
                    LeagueId = league.Id,
                    Number = globalMatchdayNumber++,
                    Matches = new List<Fut7Match>(),
                    RestingTeamNames = new List<string>()
                };

                foreach (var groupSchedule in groupSchedules) {

                    if (i < groupSchedule.Count) {

                        var sourceMatchday = groupSchedule[i];

                        foreach (var match in sourceMatchday.Matches) {
                            matchday.Matches.Add(match);
                        }

                        matchday.RestingTeamNames
                            .AddRange(sourceMatchday.RestingTeamNames);
                    }
                }

                result.Add(matchday);
            }

            return result;
        }

        // =========================
        // 🔹 INTER GROUP
        // =========================

        private List<Matchday> GenerateInterGroup(League league) {

            var allTeams = league.Groups
                .SelectMany(g => g.Teams)
                .ToList();

            return RoundRobin(
                allTeams,
                league.Id,
                null,
                1);
        }

        // =========================
        // 🔹 ROUND ROBIN CORE
        // =========================

        private List<Matchday> RoundRobin(
            List<Team> teams,
            int leagueId,
            int? groupId,
            int startMatchdayNumber) {

            var matchdays = new List<Matchday>();

            var workingTeams = teams
                .OrderBy(x => _random.Next())
                .ToList();

            var homeCounts = new Dictionary<int, int>();
            var awayCounts = new Dictionary<int, int>();

            // 🔹 Si es impar → agregar BYE
            if (workingTeams.Count % 2 != 0)
                workingTeams.Add(new Team {
                    Id = -1,
                    Name = "BYE"
                });

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

                    var away =
                        workingTeams[workingTeams.Count - 1 - match];

                    // 🔹 Detectar descanso

                    if (home.Id == -1 && away.Id != -1) {

                        matchday.RestingTeamNames
                            .Add(away.Name ?? $"Equipo {away.Id}");

                        continue;
                    }

                    if (away.Id == -1 && home.Id != -1) {

                        matchday.RestingTeamNames
                            .Add(home.Name ?? $"Equipo {home.Id}");

                        continue;
                    }

                    if (home.Id == -1 || away.Id == -1)
                        continue;

                    // 🔹 Inicializar contadores

                    if (!homeCounts.ContainsKey(home.Id))
                        homeCounts[home.Id] = 0;

                    if (!homeCounts.ContainsKey(away.Id))
                        homeCounts[away.Id] = 0;

                    if (!awayCounts.ContainsKey(home.Id))
                        awayCounts[home.Id] = 0;

                    if (!awayCounts.ContainsKey(away.Id))
                        awayCounts[away.Id] = 0;

                    // 🔹 Balancear local/visitante

                    int homeDiff =
                        homeCounts[home.Id] - awayCounts[home.Id];

                    int awayDiff =
                        homeCounts[away.Id] - awayCounts[away.Id];

                    // 🔹 Invertir si home tiene demasiados locales

                    if (homeDiff > awayDiff) {
                        (home, away) = (away, home);
                    }

                    var finalGroupId = groupId ?? home.GroupId;

                    if (finalGroupId == null)
                        throw new Exception("No se pudo determinar el grupo.");

                    matchday.Matches.Add(new Fut7Match {

                        LeagueId = leagueId,
                        GroupId = finalGroupId.Value,

                        HomeTeamId = home.Id,
                        AwayTeamId = away.Id
                    });

                    // 🔹 Actualizar contadores

                    homeCounts[home.Id]++;
                    awayCounts[away.Id]++;
                }

                matchdays.Add(matchday);

                // 🔹 Rotación round robin

                var last = workingTeams[^1];

                workingTeams.RemoveAt(workingTeams.Count - 1);

                workingTeams.Insert(1, last);
            }

            return matchdays;
        }

        public async Task<LeagueDashboardDto> GetDashboardAsync(int leagueId) {
            // =========================
            // 🔹 Jornada actual
            // =========================
            var league = await _context.Leagues.FirstAsync(l => l.Id == leagueId);


            var currentMatchday = await _context.Matchdays
                .Include(md => md.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(md => md.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .Where(md => md.LeagueId == leagueId)
                .OrderBy(md => md.Number)
                .FirstOrDefaultAsync(md =>
                    md.Matches.Any(m =>
                        m.HomeGoals == null ||
                        m.AwayGoals == null));

            MatchdayDto? matchdayDto = null;

            if (currentMatchday != null) {
                var allTeams = await _context.Teams
                    .Where(t => t.LeagueId == leagueId)
                    .ToListAsync();

                currentMatchday.RestingTeamNames = allTeams
                    .Where(t => !currentMatchday.Matches.Any(m =>
                        m.HomeTeamId == t.Id ||
                        m.AwayTeamId == t.Id))
                    .Select(t => t.Name)
                    .ToList();

                matchdayDto = _mapper.Map<MatchdayDto>(currentMatchday);
            }

            // =========================
            // 🔹 Equipos
            // =========================

            var teams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();


            // =========================
            // 🔹 Partidos jugados
            // =========================

            var matches = await _context.Matches
                .Where(m =>
                    m.LeagueId == leagueId &&
                    m.HomeGoals != null &&
                    m.AwayGoals != null)
                .OrderByDescending(m => m.MatchDate.HasValue)
                .ThenByDescending(m => m.MatchDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();

            // =========================
            // 🔹 Grupos
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
                        Last5Results = new List<string>(),
                        LogoUrl = t.LogoUrl
                    });

                foreach (var m in matches) {

                    var homeExists =
                        standingsDict.ContainsKey(m.HomeTeamId);

                    var awayExists =
                        standingsDict.ContainsKey(m.AwayTeamId);

                    // este grupo no participa
                    if (!homeExists && !awayExists)
                        continue;

                    StandingDto? home = null;
                    StandingDto? away = null;

                    if (homeExists)
                        home = standingsDict[m.HomeTeamId];

                    if (awayExists)
                        away = standingsDict[m.AwayTeamId];

                    // =========================
                    // PARTIDOS JUGADOS
                    // =========================

                    if (home != null) {

                        home.Played++;
                        home.GoalsFor += m.HomeGoals!.Value;
                        home.GoalsAgainst += m.AwayGoals!.Value;
                    }

                    if (away != null) {

                        away.Played++;
                        away.GoalsFor += m.AwayGoals!.Value;
                        away.GoalsAgainst += m.HomeGoals!.Value;
                    }

                    // =========================
                    // RESULTADO
                    // =========================

                    if (m.HomeGoals > m.AwayGoals) {

                        if (home != null) {
                            home.Won++;
                            home.Points += 3;
                            AddResult(home, "W");
                        }

                        if (away != null) {
                            away.Lost++;
                            AddResult(away, "L");
                        }

                    } else if (m.HomeGoals < m.AwayGoals) {

                        if (away != null) {
                            away.Won++;
                            away.Points += 3;
                            AddResult(away, "W");
                        }

                        if (home != null) {
                            home.Lost++;
                            AddResult(home, "L");
                        }

                    } else { //empate
                        if (league.UsePenaltyShootoutPoints) {
                            if (home != null && away != null) {
                                home.Draw++;
                                away.Draw++;

                                var homePenaltyGoals = m.HomePenaltyGoals ?? 0;
                                var awayPenaltyGoals = m.AwayPenaltyGoals ?? 0;


                                if (homePenaltyGoals > awayPenaltyGoals) {
                                    home.Points += 2;
                                    away.Points++;
                                } else if (homePenaltyGoals < awayPenaltyGoals) {
                                    home.Points++;
                                    away.Points += 2;
                                } else {
                                    home.Points++;
                                    away.Points++;   
                                }

                                AddResult(home, "D");
                                AddResult(away, "D");
                            }
                        } else {

                            if (home != null && away != null) {
                                home.Draw++;
                                home.Points++;
                                AddResult(home, "D");
                                away.Draw++;
                                away.Points++;
                                AddResult(away, "D");
                            }
                        }
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

                    GroupName =
                        groups.FirstOrDefault(g => g.Id == groupId)?.Name
                        ?? $"Grupo {groupId}",

                    Standings = standings
                });
            }

            // =========================
            // 🔹 Tabla general
            // =========================

            var flatStandings = groupedStandings
                 .SelectMany(g => g.Standings)
                 .Select(s => new StandingDto {

                     TeamId = s.TeamId,
                     TeamName = s.TeamName,
                     Played = s.Played,
                     Won = s.Won,
                     Draw = s.Draw,
                     Lost = s.Lost,
                     GoalsFor = s.GoalsFor,
                     GoalsAgainst = s.GoalsAgainst,
                     Points = s.Points,
                     Last5Results = s.Last5Results.ToList(),
                     LogoUrl = s.LogoUrl
                 })
                 .OrderByDescending(t => t.Points)
                 .ThenByDescending(t => t.GoalDifference)
                 .ThenByDescending(t => t.GoalsFor)
                 .ThenBy(t => t.TeamName)
                 .ToList();

            for (int i = 0; i < flatStandings.Count; i++)
                flatStandings[i].Position = i + 1;

            return new LeagueDashboardDto {
                CurrentMatchday = matchdayDto,
                GroupedStandings = groupedStandings,
                Standings = flatStandings
            };
        }

        public async Task<StandingsResponseDto> GetStandingsAsync(int leagueId) {
            var league = await _context.Leagues.FirstAsync(l => l.Id == leagueId);

            var teams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();

            var matches = await _context.Matches
                .Where(m =>
                    m.LeagueId == leagueId &&
                    m.HomeGoals != null &&
                    m.AwayGoals != null)
                .OrderByDescending(m => m.MatchDate.HasValue)
                .ThenByDescending(m => m.MatchDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();

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
                        Last5Results = new List<string>(),
                        LogoUrl = t.LogoUrl
                    });

           

                foreach (var m in matches) {

                    var homeExists =
                        standingsDict.ContainsKey(m.HomeTeamId);

                    var awayExists =
                        standingsDict.ContainsKey(m.AwayTeamId);

                    // este grupo no participa
                    if (!homeExists && !awayExists)
                        continue;

                    StandingDto? home = null;
                    StandingDto? away = null;

                    if (homeExists)
                        home = standingsDict[m.HomeTeamId];

                    if (awayExists)
                        away = standingsDict[m.AwayTeamId];

                    // =========================
                    // PARTIDOS JUGADOS
                    // =========================

                    if (home != null) {

                        home.Played++;
                        home.GoalsFor += m.HomeGoals!.Value;
                        home.GoalsAgainst += m.AwayGoals!.Value;
                    }

                    if (away != null) {

                        away.Played++;
                        away.GoalsFor += m.AwayGoals!.Value;
                        away.GoalsAgainst += m.HomeGoals!.Value;
                    }

                    // =========================
                    // RESULTADO
                    // =========================

                    if (m.HomeGoals > m.AwayGoals) {

                        if (home != null) {
                            home.Won++;
                            home.Points += 3;
                            AddResult(home, "W");
                        }

                        if (away != null) {
                            away.Lost++;
                            AddResult(away, "L");
                        }
                    } else if (m.HomeGoals < m.AwayGoals) {

                        if (away != null) {
                            away.Won++;
                            away.Points += 3;
                            AddResult(away, "W");
                        }

                        if (home != null) {
                            home.Lost++;
                            AddResult(home, "L");
                        }
                    } else {

                        if (league.UsePenaltyShootoutPoints) {
                            if (home != null && away != null) {
                                home.Draw++;
                                away.Draw++;

                                var homePenaltyGoals = m.HomePenaltyGoals ?? 0;
                                var awayPenaltyGoals = m.AwayPenaltyGoals ?? 0;


                                if (homePenaltyGoals > awayPenaltyGoals) {
                                    home.Points += 2;
                                    away.Points++;
                                } else if (homePenaltyGoals < awayPenaltyGoals) {
                                    home.Points++;
                                    away.Points += 2;
                                } else {
                                    home.Points++;
                                    away.Points++;
                                }

                                AddResult(home, "D");
                                AddResult(away, "D");
                            }
                        } else {

                            if (home != null && away != null) {
                                home.Draw++;
                                home.Points++;
                                AddResult(home, "D");
                                away.Draw++;
                                away.Points++;
                                AddResult(away, "D");
                            }
                        }
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

                    GroupName =
                        groups.FirstOrDefault(g => g.Id == groupId)?.Name
                        ?? $"Grupo {groupId}",

                    Standings = standings
                });
            }

            // =========================
            // 🔹 Tabla general
            // =========================

            var generalStandings = groupedStandings
                .SelectMany(g => g.Standings)
                .Select(s => new StandingDto {

                    TeamId = s.TeamId,
                    TeamName = s.TeamName,
                    Played = s.Played,
                    Won = s.Won,
                    Draw = s.Draw,
                    Lost = s.Lost,
                    GoalsFor = s.GoalsFor,
                    GoalsAgainst = s.GoalsAgainst,
                    Points = s.Points,
                    Last5Results = s.Last5Results.ToList(),
                    LogoUrl = s.LogoUrl
                })
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

        private static void AddResult(StandingDto standing, string result) {

            standing.Last5Results.Insert(0, result);

            if (standing.Last5Results.Count > 5)
                standing.Last5Results.RemoveAt(5);
        }
    }
}