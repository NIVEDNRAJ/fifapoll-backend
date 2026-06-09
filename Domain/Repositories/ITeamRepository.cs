using System.Collections.Generic;
using System.Threading.Tasks;
using FifaPollApi.Domain.Entities;

namespace FifaPollApi.Domain.Repositories
{
    public interface ITeamRepository
    {
        Task<Team?> GetByIdAsync(int id);
        Task<Team?> GetByNameAsync(string teamName);
        Task<IEnumerable<Team>> GetAllAsync();
        Task AddAsync(Team team);
        void Update(Team team);
        void Delete(Team team);
        Task<bool> ExistsByNameAsync(string teamName);
    }
}
