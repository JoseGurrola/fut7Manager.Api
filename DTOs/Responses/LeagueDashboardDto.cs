namespace fut7Manager.Api.DTOs.Responses {
    public class LeagueDashboardDto {
        public MatchdayDto CurrentMatchday { get; set; }

        public List<GroupStandingDto> GroupedStandings { get; set; }

        public List<StandingDto> Standings { get; set; } // fallback
    }
}
