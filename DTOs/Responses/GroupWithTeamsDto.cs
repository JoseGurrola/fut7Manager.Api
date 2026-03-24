namespace fut7Manager.Api.DTOs.Responses {
    public class GroupWithTeamsDto {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int LeagueId { get; set; }
        public List<TeamDto> Teams { get; set; } = new();
    }
}
