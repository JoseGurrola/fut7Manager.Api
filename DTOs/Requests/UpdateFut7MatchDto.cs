namespace fut7Manager.Api.DTOs.Requests {
    public class UpdateFut7MatchDto {
        public int HomeGoals { get; set; }

        public int AwayGoals { get; set; }

        public DateTime MatchDate { get; set; }

        public string? Location { get; set; }
    }
}
