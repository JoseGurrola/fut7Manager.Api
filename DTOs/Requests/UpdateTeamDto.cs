using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Requests {
    public class UpdateTeamDto {
        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        public int PositionTable { get; set; }

        public int Points { get; set; }

        public int GoalsFor { get; set; }

        [Required]
        public string TeamManagerName { get; set; } = default!;

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string TeamManagerPhone { get; set; } = default!;

        public int GoalsAgainst { get; set; }

        public int GroupId { get; set; }

       
    }
}
