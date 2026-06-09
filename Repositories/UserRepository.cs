using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FifaPollApi.Data;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;

namespace FifaPollApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FifaPollDbContext _context;

        public UserRepository(FifaPollDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
