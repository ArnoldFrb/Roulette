using Microsoft.EntityFrameworkCore;
using Roulette.Domain.Entities;

namespace Roulette.Infrastructure
{
    public class RouletteDbContext(DbContextOptions<RouletteDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RouletteEntity> Roulettes { get; set; }
        public DbSet<BetEntity> Bets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Credit).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.IsAdmin).IsRequired();
            });

            modelBuilder.Entity<RouletteEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.ColorWinner).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.NumberWinner).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.OpenedAt).IsRequired();
                entity.Property(e => e.ClosedAt).IsRequired();
            });

            modelBuilder.Entity<BetEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.BetType).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.Color).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.Number).IsRequired();

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.User)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Roulette)
                      .WithMany()
                      .HasForeignKey(e => e.Roulette)
                      .OnDelete(DeleteBehavior.Cascade);

                SeedUser(modelBuilder);
            });
        }

        protected static void SeedUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity("admin", "adminpass", 1000m, true) { Id = 1 },
                new UserEntity("user1", "user1pass", 500m) { Id = 2 },
                new UserEntity("user2", "user2pass", 300m) { Id = 3 },
                new UserEntity("user3", "user3pass", 200m) { Id = 4 }
            );
        }
    }
}
