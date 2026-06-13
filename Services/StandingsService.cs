using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services.Interfaces;
using Serilog;
using Serilog.Core;
using GroupModel = fut7Manager.Api.Models.Group;

namespace fut7Manager.Api.Services {
    public class StandingsService : IStandingsService {
        private readonly StandingsEngine _engine;
        private readonly StandingsQuery _query;

        public StandingsService(
            StandingsEngine engine,
            StandingsQuery query) {
            _engine = engine;
            _query = query;
        }

        private List<GroupStandingDto> BuildGroupedStandings(
    League league,
    List<Team> teams,
    List<Fut7Match> matches,
    List<GroupModel> groups) {
            var generalStandings = _engine.Build(
                teams,
                matches.Where(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue).ToList(),
                league.UsePenaltyShootoutPoints);

            var result = new List<GroupStandingDto>();

            var teamsByGroup = teams
                .Where(t => t.GroupId != null)
                .GroupBy(t => t.GroupId!.Value);

            foreach (var group in teamsByGroup) {
                var groupId = group.Key;

                var teamIds = group
                    .Select(t => t.Id)
                    .ToHashSet();

                var standings = Map(
                    generalStandings
                        .Where(s => teamIds.Contains(s.TeamId))
                        .OrderByDescending(x => x.Points)
                        .ThenByDescending(x => x.GoalDifference)
                        .ThenByDescending(x => x.GoalsFor)
                        .ThenBy(x => x.TeamName)
                        .ToList());

                for (int i = 0; i < standings.Count; i++) {
                    standings[i].Position = i + 1;
                }

                result.Add(new GroupStandingDto {
                    GroupName = groups.FirstOrDefault(g => g.Id == groupId)?.Name
                                ?? $"Grupo {groupId}",
                    Standings = standings
                });
            }

            // 🔹 Lógica de clasificación dinámica
            int totalQualified = league.TotalQualifiedTeams ?? 0;
            int groupCount = result.Count;

            if (groupCount > 0 && totalQualified > 0) {
                int baseQualifiedPerGroup = totalQualified / groupCount;
                int remainingSlots = totalQualified % groupCount;

                // Clasificados base por grupo
                foreach (var g in result) {
                    for (int i = 0; i < g.Standings.Count; i++) {
                        if (i < baseQualifiedPerGroup) {
                            g.Standings[i].IsQualified = true;
                        }
                    }
                }

                // 🔹 Candidatos extra (máximo 1 por grupo)
                var extraCandidates = new List<StandingDto>();
                foreach (var g in result) {
                    var candidate = g.Standings
                        .Where(s => !s.IsQualified)
                        .OrderByDescending(s => s.Points)
                        .ThenByDescending(s => s.GoalDifference)
                        .ThenByDescending(s => s.GoalsFor)
                        .FirstOrDefault();

                    if (candidate != null) {
                        extraCandidates.Add(candidate);
                    }
                }

                // Selección de los mejores extras hasta llenar remainingSlots
                foreach (var team in extraCandidates
                    .OrderByDescending(s => s.Points)
                    .ThenByDescending(s => s.GoalDifference)
                    .ThenByDescending(s => s.GoalsFor)
                    .Take(remainingSlots)) {
                    team.IsQualified = true;
                }
            }

            return result;
        }


        public LeagueDashboardDto BuildDashboard(
    League league,
    List<Team> teams,
    List<Fut7Match> matches,
    List<GroupModel> groups) {
            var generalAcc = _engine.Build(
                teams,
                matches.Where(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue).ToList(),
                league.UsePenaltyShootoutPoints);

            var general = Map(generalAcc);

            for (int i = 0; i < general.Count; i++) {
                general[i].Position = i + 1;
            }

            // 🔹 Construir standings por grupo con lógica de clasificados
            var grouped = BuildGroupedStandings(league, teams, matches, groups);

            // 🔹 Sincronizar clasificados en la tabla general
            var qualifiedIds = grouped
                .SelectMany(g => g.Standings)
                .Where(s => s.IsQualified)
                .Select(s => s.TeamId)
                .ToHashSet();

            foreach (var team in general) {
                if (qualifiedIds.Contains(team.TeamId)) {
                    team.IsQualified = true;
                }
            }

            return new LeagueDashboardDto {
                GroupedStandings = grouped,
                Standings = general
            };
        }


        private static StandingDto Map(StandingAccumulator s) {
            return new StandingDto {
                TeamId = s.TeamId,
                TeamName = s.TeamName,
                LogoUrl = s.LogoUrl,
                Played = s.Played,
                Won = s.Won,
                Draw = s.Draw,
                Lost = s.Lost,
                GoalsFor = s.GoalsFor,
                GoalsAgainst = s.GoalsAgainst,
                Points = s.Points,
                Last5Results = s.Last5Results.ToList()
            };
        }

        private static List<StandingDto> Map(List<StandingAccumulator> list) {
            return list.Select(Map).ToList();
        }

    }
}