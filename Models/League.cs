using fut7Manager.Api.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace fut7Manager.Api.Models {
    public class League {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;
        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();

        public ICollection<Fut7Match> Matches { get; set; } = new List<Fut7Match>();

        public ICollection<Matchday> Matchdays { get; set; } = new List<Matchday>();

        public bool IsScheduleGenerated { get; set; }
        public bool InterGroupMatches { get; set; }  // true = A vs B permitido

        public decimal RegistrationFee { get; set; }  // costo de inscripción

        public LeagueStatus Status { get; set; } = LeagueStatus.Upcoming;
    }
}
