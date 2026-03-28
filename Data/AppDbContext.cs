using fut7Manager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace fut7Manager.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<League> Leagues { get; set; }

        public DbSet<Fut7Match> Matches { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Matchday> Matchdays { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<League>()
                .Property(l => l.RegistrationFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Fut7Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fut7Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fut7Match>()
                .HasOne(m => m.Group)
                .WithMany()
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.League)
                .WithMany(l => l.Groups)
                .HasForeignKey(g => g.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Group)
                .WithMany(g => g.Teams)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matchday>()
                .HasOne(md => md.League)
                .WithMany(l => l.Matchdays)
                .HasForeignKey(md => md.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Payments)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}