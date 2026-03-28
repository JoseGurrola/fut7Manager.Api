namespace fut7Manager.Api.Models {
    public class Payment {
        public int Id { get; set; }

        public int TeamId { get; set; }   // FK obligatoria
        public Team Team { get; set; } = null!;   // navegación

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
