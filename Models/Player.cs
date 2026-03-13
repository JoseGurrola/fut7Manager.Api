using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.Models {
    public enum PlayerPosition {
        Goalkeeper,
        Defender,
        Midfielder,
        Forward
    }
    public class Player {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        public int JerseyNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public PlayerPosition Position { get; set; } = default!;

        public int Goals { get; set; }

        public int MatchesPlayed { get; set; }

        public bool Active { get; set; }

        // Foreign Key
        public int TeamId { get; set; }

        // Navigation Property
        public Team Team { get; set; } = default!;
    }
}
