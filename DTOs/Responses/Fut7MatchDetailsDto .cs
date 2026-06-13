namespace fut7Manager.Api.DTOs.Responses {
    public class Fut7MatchDetailsDto {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public string HomeTeamName { get; set; } = default!;

        public string AwayTeamName { get; set; } = default!;

        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? Location { get; set; }

        public int? MatchdayId { get; set; }
        public int GroupId { get; set; }
        public int LeagueId { get; set; }

        public int MatchdayNumber { get; set; }

        public string? HomeTeamLogo { get; set; }
        public string? AwayTeamLogo { get; set; }

        public string HomeTeamPrimaryColor { get; set; } = default!;
        public string AwayTeamPrimaryColor { get; set; } = default!;

        public int? HomePenaltyGoals { get; set; }

        public int? AwayPenaltyGoals { get; set; }

        public List<MatchPlayerStatDto> HomePlayerStats { get; set; } = [];

        public List<MatchPlayerStatDto> AwayPlayerStats { get; set; } = [];
    }
}
