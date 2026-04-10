using System.ComponentModel.DataAnnotations.Schema;

namespace fut7Manager.Api.DTOs.Responses {
    public class MatchdayDto {
        public int Id { get; set; }
        public int Number { get; set; }
        public List<Fut7MatchDto> Matches { get; set; } = new();
        public List<string> RestingTeamNames { get; set; } = new();
    }
}
