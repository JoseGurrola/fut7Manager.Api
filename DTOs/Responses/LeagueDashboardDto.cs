namespace fut7Manager.Api.DTOs.Responses {
    public class LeagueDashboardDto {
        public MatchdayDto? CurrentMatchday { get; set; }

        public List<GroupStandingDto> GroupedStandings { get; set; } = new();

        public List<StandingDto> Standings { get; set; } = new();

        public PlayerStandingsDto PlayerStandings { get; set; } = new();
    }
}
