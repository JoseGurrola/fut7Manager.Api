namespace fut7Manager.Api.DTOs.Requests {
    public class UpdateTeamDto {
        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        public int PositionTable { get; set; }

        public int Points { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }
    }
}
