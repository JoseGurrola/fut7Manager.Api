using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Requests {
    public class CreatePaymentDto {
        [Required]
        public int TeamId { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }
    }
}
