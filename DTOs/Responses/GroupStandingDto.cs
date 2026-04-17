namespace fut7Manager.Api.DTOs.Responses {
    public class GroupStandingDto {
        public string GroupName { get; set; } = "";
        public List<StandingDto> Standings { get; set; } = new();
    }
}
