namespace fut7Manager.Api.DTOs.Responses {
    public class PlayerStatStandingDto {
        public int PlayerId { get; set; }
        public int Position { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int JerseyNumber { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string? TeamLogoUrl { get; set; }
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}
