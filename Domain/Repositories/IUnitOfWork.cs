using System;
using System.Threading.Tasks;

namespace FifaPollApi.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITeamRepository Teams { get; }
        IVoteRepository Votes { get; }
        ISettingRepository Settings { get; }
        Task<int> CompleteAsync();
    }
}
