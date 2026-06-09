using System.Threading.Tasks;
using FifaPollApi.Data;
using FifaPollApi.Domain.Repositories;

namespace FifaPollApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FifaPollDbContext _context;

        public IUserRepository Users { get; }
        public ITeamRepository Teams { get; }
        public IVoteRepository Votes { get; }
        public ISettingRepository Settings { get; }

        public UnitOfWork(
            FifaPollDbContext context,
            IUserRepository users,
            ITeamRepository teams,
            IVoteRepository votes,
            ISettingRepository settings)
        {
            _context = context;
            Users = users;
            Teams = teams;
            Votes = votes;
            Settings = settings;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
