namespace fut7Manager.Api.DTOs.Responses {
    public class StandingsResponseDto {
        public MatchdayDto CurrentMatchday { get; set; } = default!;
        public List<GroupStandingDto> GroupedStandings { get; set; } = new();
        public List<StandingDto> Standings { get; set; } = new();
    }
}
