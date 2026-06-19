namespace fut7Manager.Api.DTOs.Responses {
    public class PlayerStandingsDto {
        public List<PlayerStatStandingDto> TopScorers { get; set; } = new();
        public List<PlayerStatStandingDto> YellowCards { get; set; } = new();
        public List<PlayerStatStandingDto> RedCards { get; set; } = new();
    }
}
