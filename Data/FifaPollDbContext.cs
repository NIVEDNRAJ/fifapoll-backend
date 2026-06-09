using Microsoft.EntityFrameworkCore;
using FifaPollApi.Domain.Entities;

namespace FifaPollApi.Data
{
    public class FifaPollDbContext : DbContext
    {
        public FifaPollDbContext(DbContextOptions<FifaPollDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Vote> Votes => Set<Vote>();
        public DbSet<Setting> Settings => Set<Setting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
            });

            // Team configuration
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TeamName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.TeamName).IsUnique();
                entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(3);
                entity.Property(e => e.FlagUrl).IsRequired().HasMaxLength(500);
            });

            // Vote configuration
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.VotedAt).IsRequired();
                
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Team)
                    .WithMany()
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Setting configuration
            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsVotingEnabled).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.IsResultPublished).IsRequired().HasDefaultValue(false);
            });
        }
    }
}
