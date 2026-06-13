using fut7Manager.Api.DTOs.Responses;

namespace fut7Manager.Api.DTOs.Requests {
    public class UpdateFut7MatchDto {
        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public DateTime MatchDate { get; set; }

        public string? Location { get; set; }

        public int? HomePenaltyGoals { get; set; }

        public int? AwayPenaltyGoals { get; set; }

        public List<MatchPlayerStatDto> HomePlayerStats { get; set; } = [];

        public List<MatchPlayerStatDto> AwayPlayerStats { get; set; } = [];
    }
}
