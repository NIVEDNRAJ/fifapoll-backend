using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FifaPollApi.Data;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;

namespace FifaPollApi.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly FifaPollDbContext _context;

        public TeamRepository(FifaPollDbContext context)
        {
            _context = context;
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task<Team?> GetByNameAsync(string teamName)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.TeamName == teamName);
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public void Update(Team team)
        {
            _context.Teams.Update(team);
        }

        public void Delete(Team team)
        {
            _context.Teams.Remove(team);
        }

        public async Task<bool> ExistsByNameAsync(string teamName)
        {
            return await _context.Teams.AnyAsync(t => t.TeamName == teamName);
        }
    }
}
