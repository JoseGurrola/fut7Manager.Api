using fut7Manager.Api.Helpers;
using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Requests {
    public class CreateLeagueDto {
        public string Name { get; set; } = default!;

        public decimal RegistrationFee { get; set; }

        public LeagueStatus Status { get; set; } = LeagueStatus.Upcoming;

        public string? LogoUrl { get; set; }

        public bool UsePenaltyShootoutPoints { get; set; }

        public int? QualifiedTeamsPerGroup { get; set; }

        public int? MinPlayers { get; set; }
    }
}
