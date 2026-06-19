using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;
using Serilog;

namespace fut7Manager.Api.Helpers {
    public class StandingsEngine {
        private readonly MatchResultApplier _applier;

        public StandingsEngine(MatchResultApplier applier) {
            _applier = applier;
        }

        public List<StandingAccumulator> Build(
    List<Team> teams,
    List<Fut7Match> matches,
    bool usePenalties) {
            var table = teams.ToDictionary(
                t => t.Id,
                t => new StandingAccumulator {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    LogoUrl = t.LogoUrl
                });

            var validMatches = matches
                .Where(m =>
                    m.HomeGoals.HasValue &&
                    m.AwayGoals.HasValue &&
                    m.HomeTeamId != m.AwayTeamId)
                .ToList();

            foreach (var match in validMatches) {
                //Log.Information(
                //    $"MATCH {match.Id} | " +
                //    $"{match.HomeTeamId} vs {match.AwayTeamId} | " +
                //    $"{match.HomeGoals}-{match.AwayGoals} | " +
                //    $"Group:{match.GroupId}");

                _applier.Apply(table, match, usePenalties);
            }
            //foreach (var row in table.Values) {
                //Log.Information(
                //    $"TEAM {row.TeamName} " +
                //    $"P:{row.Played} W:{row.Won} D:{row.Draw} L:{row.Lost} PTS:{row.Points}");
            //}
            return table.Values
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsFor)
                .ThenBy(x => x.TeamName)
                .ToList();
        }
    }
}
