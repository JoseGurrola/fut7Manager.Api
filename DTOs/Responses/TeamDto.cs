namespace fut7Manager.Api.DTOs.Responses {
    public class TeamDto {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        public int PositionTable { get; set; }

        public int Points { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }

        public int LeagueId { get; set; }

        public int GroupId { get; set; }

        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
    }
}
