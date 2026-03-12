using System.ComponentModel.DataAnnotations;

namespace fut7Manager.DTOs.Requests {
    public class LoginRequestDto {
        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
