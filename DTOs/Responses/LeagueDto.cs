using fut7Manager.Api.Helpers;
using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Responses {
    public class LeagueDto {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public decimal RegistrationFee { get; set; }

        public LeagueStatus Status { get; set; }

        public string? LogoUrl { get; set; }

        public DateTime CreationDate { get; set; }

        public bool UsePenaltyShootoutPoints { get; set; }

        public int? TotalQualifiedTeams { get; set; }

        public int? MinPlayers { get; set; }
    }
}