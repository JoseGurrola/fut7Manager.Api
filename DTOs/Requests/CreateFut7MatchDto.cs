namespace fut7Manager.Api.DTOs.Requests {
    public class CreateFut7MatchDto {
        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public DateTime MatchDate { get; set; }

        public string? Location { get; set; }

        public int LeagueId { get; set; }
    }
}
