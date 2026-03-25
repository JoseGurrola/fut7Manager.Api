using System.Text.RegularExpressions;

namespace fut7Manager.Api.Models {
    public class Matchday {
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public int Number { get; set; }

        public League League { get; set; } = default!;
        public ICollection<Fut7Match> Matches { get; set; } = new List<Fut7Match>();
    }
}
