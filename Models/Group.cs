using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.Models {
    public class Group {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = default!;

        // Foreign key
        public int LeagueId { get; set; }

        // Navigation properties
        public League League { get; set; } = default!;

        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}