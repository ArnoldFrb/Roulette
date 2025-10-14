using Microsoft.EntityFrameworkCore;
using Roulette.Domain.Entities;

namespace Roulette.Infrastructure.Data
{
    public class RouletteDbContext(DbContextOptions<RouletteDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<GamblerEntity> Gamblers { get; set; }
        public DbSet<CrupierEntity> Crupieres { get; set; }
        public DbSet<RouletteEntity> Roulettes { get; set; }
        public DbSet<BetEntity> Bets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>().ToTable("User");
            modelBuilder.Entity<CrupierEntity>().ToTable("Crupier");
            modelBuilder.Entity<GamblerEntity>().ToTable("Gambler");

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<CrupierEntity>(entity => entity.Property(e => e.Password).IsRequired().HasMaxLength(100));

            modelBuilder.Entity<GamblerEntity>(entity =>
            {
                entity.Property(e => e.Credit).IsRequired().HasPrecision(10, 2);

                entity.HasMany(e => e.Bets)
                      .WithOne(e => e.Gambler)
                      .HasForeignKey(e => e.GamblerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RouletteEntity>(entity =>
            {
                entity.ToTable("Roulette");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.ColorWinner).HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.NumberWinner);
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.OpenedAt);
                entity.Property(e => e.ClosedAt);

                entity.HasMany(e => e.Bets)
                      .WithOne(e => e.Roulette)
                      .HasForeignKey(e => e.RouletteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BetEntity>(entity =>
            {
                entity.ToTable("Bet");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.BetType).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.Color).HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.Number);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Winnings).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.Result).IsRequired().HasConversion<string>().HasMaxLength(10);
            });

            SeedCrupier(modelBuilder);
            SeedGambler(modelBuilder);
        }

        protected static void SeedCrupier(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrupierEntity>().HasData(
                new CrupierEntity("crupier1", "password1") { Id = 1 }
            );
        }

        protected static void SeedGambler(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GamblerEntity>().HasData(
                new GamblerEntity("user0", 1000m) { Id = 2 },
                new GamblerEntity("user1", 500m) { Id = 3 },
                new GamblerEntity("user2", 300m) { Id = 4 },
                new GamblerEntity("user3", 200m) { Id = 5 }
            );
        }
    }
}
