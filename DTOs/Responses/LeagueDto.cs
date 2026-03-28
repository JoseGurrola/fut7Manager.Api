namespace fut7Manager.Api.DTOs.Responses {
    public class LeagueDto {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public decimal RegistrationFee { get; set; }
    }
}
