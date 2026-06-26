namespace fut7Manager.Api.DTOs.Responses {
    public class PaymentSummaryDto {
        public decimal TotalDue { get; set; }      // Total que deberían pagar todos los equipos
        public decimal TotalPaid { get; set; }     // Total pagado por todos los equipos
        public decimal PercentagePaid { get; set; } // Porcentaje global pagado
    }
}
