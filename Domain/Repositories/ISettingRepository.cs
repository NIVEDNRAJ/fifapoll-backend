using System.Threading.Tasks;
using FifaPollApi.Domain.Entities;

namespace FifaPollApi.Domain.Repositories
{
    public interface ISettingRepository
    {
        Task<Setting?> GetSettingsAsync();
        void Update(Setting setting);
    }
}
