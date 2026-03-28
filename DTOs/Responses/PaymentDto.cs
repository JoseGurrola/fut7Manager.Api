using fut7Manager.Api.Models;

namespace fut7Manager.Api.DTOs.Responses {
    public class PaymentDto {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
