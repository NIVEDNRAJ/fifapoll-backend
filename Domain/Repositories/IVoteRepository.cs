using System.Collections.Generic;
using System.Threading.Tasks;
using FifaPollApi.Domain.Entities;

namespace FifaPollApi.Domain.Repositories
{
    public interface IVoteRepository
    {
        Task<Vote?> GetByIdAsync(int id);
        Task<Vote?> GetActiveVoteByUserIdAsync(int userId);
        Task<IEnumerable<Vote>> GetAllActiveVotesAsync();
        Task<IEnumerable<Vote>> GetAllVotesAsync();
        Task AddAsync(Vote vote);
        void Update(Vote vote);
    }
}
