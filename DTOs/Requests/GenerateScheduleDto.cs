namespace fut7Manager.Api.DTOs.Requests {
    public class GenerateScheduleDto {

        public bool InterGroupMatches { get; set; }

        public List<TeamGroupAssignmentDto> Teams { get; set; } = [];
    }
}
