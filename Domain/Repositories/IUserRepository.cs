using System.Collections.Generic;
using System.Threading.Tasks;
using FifaPollApi.Domain.Entities;

namespace FifaPollApi.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
