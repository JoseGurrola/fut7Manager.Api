namespace fut7Manager.Api.Models {
    public class Fut7Match {
        public int Id { get; set; }

        public int? MatchdayId { get; set; }
        public Matchday? Matchday { get; set; } = default!;
        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? Location { get; set; }

        public int LeagueId { get; set; }

        // Navigation properties
        public Team HomeTeam { get; set; } = default!;

        public Team AwayTeam { get; set; } = default!;

        public int GroupId { get; set; }
        public Group Group { get; set; } = default!;

        public League League { get; set; } = default!;
    }
}
