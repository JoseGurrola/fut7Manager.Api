using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.DTOs.Requests {
    public class CreateLeagueDto {
        public string Name { get; set; } = default!;

        public decimal RegistrationFee { get; set; }

        public LeagueStatus Status { get; set; } = LeagueStatus.Upcoming;
    }
}
