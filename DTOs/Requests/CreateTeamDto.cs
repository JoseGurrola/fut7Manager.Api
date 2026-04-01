using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Requests {
    public class CreateTeamDto {
        [Required]
        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        [Required]
        public string TeamManagerName { get; set; } = default!;

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string TeamManagerPhone { get; set; } = default!;

        public int LeagueId { get; set; }

        public int GroupId { get; set; }
       
    }
}
