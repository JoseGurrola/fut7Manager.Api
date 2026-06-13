namespace fut7Manager.Api.Models {
    public class MatchPlayerStat {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int? PlayerId { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public int? JerseyNumber { get; set; }

        public bool IsHomeTeam { get; set; }

        public int Goals { get; set; }

        public int YellowCards { get; set; }

        public int RedCards { get; set; }

        public Fut7Match Match { get; set; } = default!;
    }
}
