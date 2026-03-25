using fut7Manager.Api.Models;

namespace fut7Manager.Api.Helpers {
    public static class SpecialFunctions {

        private static List<Matchday> RoundRobin(List<Team> teams, int leagueId, int groupId) {
            var matchdays = new List<Matchday>();

            if (teams.Count % 2 != 0)
                teams.Add(new Team { Id = -1 }); // bye

            int numMatchdays = teams.Count - 1;
            int matchesPerDay = teams.Count / 2;

            for (int day = 0; day < numMatchdays; day++) {
                var matchday = new Matchday {
                    LeagueId = leagueId,
                    Number = day + 1
                };

                for (int match = 0; match < matchesPerDay; match++) {
                    var home = teams[match];
                    var away = teams[teams.Count - 1 - match];

                    if (home.Id == -1 || away.Id == -1)
                        continue;

                    matchday.Matches.Add(new Fut7Match {
                        LeagueId = leagueId,
                        GroupId = groupId,
                        HomeTeamId = home.Id,
                        AwayTeamId = away.Id
                    });
                }

                matchdays.Add(matchday);

                // rotación
                var last = teams[^1];
                teams.RemoveAt(teams.Count - 1);
                teams.Insert(1, last);
            }

            return matchdays;
        }
    }
}
