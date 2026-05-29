using fut7Manager.Api.Models;

namespace fut7Manager.Api.Helpers {
    public class StandingsQuery {
        public List<Fut7Match> FilterByGroup(
            List<Fut7Match> matches,
            int groupId) {
            return matches
                .Where(m => m.GroupId == groupId)
                .ToList();
        }

        public List<Fut7Match> InterGroup(List<Fut7Match> matches) {
            return matches
                .Where(m => m.GroupId == null)
                .ToList();
        }

        public List<Fut7Match> AllFinished(List<Fut7Match> matches) {
            return matches
                .Where(m => m.HomeGoals != null && m.AwayGoals != null)
                .ToList();
        }
    }
}
