namespace fut7Manager.Api.DTOs.Requests {
    public class CreateTeamDto {
        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        public int LeagueId { get; set; }
    }
}
