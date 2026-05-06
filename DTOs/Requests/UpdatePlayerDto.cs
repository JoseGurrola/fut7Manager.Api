using fut7Manager.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Requests {
    public class UpdatePlayerDto {
        [Required]
        public string Name { get; set; } = default!;

        public int JerseyNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Phone { get; set; }

        public PlayerPosition Position { get; set; } = default!;

        public bool Active { get; set; }

        public int TeamId { get; set; }
    }
}
