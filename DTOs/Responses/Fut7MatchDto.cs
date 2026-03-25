namespace fut7Manager.Api.DTOs.Responses {
    public class Fut7MatchDto {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public string HomeTeamName { get; set; } = default!;

        public string AwayTeamName { get; set; } = default!;

        //public int HomeGoals { get; set; }

        //public int AwayGoals { get; set; }

        //public DateTime MatchDate { get; set; }

        //public string? Location { get; set; }

        public int? MatchdayId { get; set; }
        public int GroupId { get; set; }
        public int LeagueId { get; set; }
    }
}
