using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.Models {
    public class Team {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        public int PositionTable { get; set; }

        public int Points { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }

        // Foreign Key
        public int LeagueId { get; set; }

        // Navigation properties
        //public League League { get; set; } = default!;

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
