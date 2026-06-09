using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Security;

namespace FifaPollApi.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(FifaPollDbContext context, IPasswordHasher passwordHasher)
        {
            // Ensure DB is created and tables exist
            await context.Database.EnsureCreatedAsync();

            // Seed Settings
            if (!await context.Settings.AnyAsync())
            {
                await context.Settings.AddAsync(new Setting
                {
                    IsVotingEnabled = true,
                    IsResultPublished = false
                });
            }

            // Seed Admin User
            if (!await context.Users.AnyAsync(u => u.Email == "admin@fifapoll.com"))
            {
                var adminUser = new User
                {
                    Name = "FIFA Admin",
                    Email = "admin@fifapoll.com",
                    PasswordHash = passwordHasher.HashPassword("Admin@1234"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };
                await context.Users.AddAsync(adminUser);
            }

            // Seed FIFA Teams
            if (!await context.Teams.AnyAsync())
            {
                var teams = new[]
                {
                    new Team { TeamName = "Argentina", CountryCode = "ARG", FlagUrl = "https://flagcdn.com/w320/ar.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Brazil", CountryCode = "BRA", FlagUrl = "https://flagcdn.com/w320/br.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "France", CountryCode = "FRA", FlagUrl = "https://flagcdn.com/w320/fr.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Germany", CountryCode = "GER", FlagUrl = "https://flagcdn.com/w320/de.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "England", CountryCode = "ENG", FlagUrl = "https://flagcdn.com/w320/gb-eng.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Spain", CountryCode = "ESP", FlagUrl = "https://flagcdn.com/w320/es.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Italy", CountryCode = "ITA", FlagUrl = "https://flagcdn.com/w320/it.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Portugal", CountryCode = "POR", FlagUrl = "https://flagcdn.com/w320/pt.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Netherlands", CountryCode = "NED", FlagUrl = "https://flagcdn.com/w320/nl.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Belgium", CountryCode = "BEL", FlagUrl = "https://flagcdn.com/w320/be.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Croatia", CountryCode = "CRO", FlagUrl = "https://flagcdn.com/w320/hr.png", CreatedAt = DateTime.UtcNow },
                    new Team { TeamName = "Morocco", CountryCode = "MAR", FlagUrl = "https://flagcdn.com/w320/ma.png", CreatedAt = DateTime.UtcNow }
                };

                await context.Teams.AddRangeAsync(teams);
            }

            await context.SaveChangesAsync();
        }
    }
}
