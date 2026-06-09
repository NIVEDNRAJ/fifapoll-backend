using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FifaPollApi.Data;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;

namespace FifaPollApi.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly FifaPollDbContext _context;

        public VoteRepository(FifaPollDbContext context)
        {
            _context = context;
        }

        public async Task<Vote?> GetByIdAsync(int id)
        {
            return await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Team)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vote?> GetActiveVoteByUserIdAsync(int userId)
        {
            return await _context.Votes
                .Include(v => v.Team)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.IsActive);
        }

        public async Task<IEnumerable<Vote>> GetAllActiveVotesAsync()
        {
            return await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Team)
                .Where(v => v.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vote>> GetAllVotesAsync()
        {
            return await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Team)
                .ToListAsync();
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
        }

        public void Update(Vote vote)
        {
            _context.Votes.Update(vote);
        }
    }
}
