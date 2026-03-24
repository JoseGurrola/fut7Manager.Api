namespace fut7Manager.Api.DTOs.Requests {
    public class CreateGroupDto {
        public string Name { get; set; } = default!;
        public int LeagueId { get; set; }
    }
}
