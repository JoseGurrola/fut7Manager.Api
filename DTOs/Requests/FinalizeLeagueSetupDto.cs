using fut7Manager.Api.DTOs.Responses;

namespace fut7Manager.Api.DTOs.Requests {
    public class FinalizeLeagueSetupDto {

        public bool InterGroupMatches { get; set; }

        public List<TeamGroupAssignmentDto> Teams { get; set; } = [];

        public List<MatchdayDto> Matchdays { get; set; } = [];
    }
}
