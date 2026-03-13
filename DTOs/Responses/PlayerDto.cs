using fut7Manager.Api.Models;

namespace fut7Manager.Api.DTOs.Responses {
    public class PlayerDto {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public int JerseyNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Phone { get; set; }

        public PlayerPosition Position { get; set; } = default!;

        public int Goals { get; set; }

        public int MatchesPlayed { get; set; }

        public bool Active { get; set; }

        public int TeamId { get; set; }
    }
}
