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

            return new LeagueDashboardDto {
                GroupedStandings = BuildGroupedStandings(
         league,
         teams,
         matches,
         groups),

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