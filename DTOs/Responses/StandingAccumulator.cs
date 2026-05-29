using fut7Manager.Api.Models;

namespace fut7Manager.Api.DTOs.Responses {
    public class StandingAccumulator {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = "";
        public string? LogoUrl { get; set; }

        public int Played { get; set; }
        public int Won { get; set; }
        public int Draw { get; set; }
        public int Lost { get; set; }

        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        public int Points { get; set; }

        public List<string> Last5Results { get; set; } = new();

        public int GoalDifference => GoalsFor - GoalsAgainst;
    }
}