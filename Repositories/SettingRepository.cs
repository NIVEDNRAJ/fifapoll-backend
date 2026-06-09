using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FifaPollApi.Data;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;

namespace FifaPollApi.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly FifaPollDbContext _context;

        public SettingRepository(FifaPollDbContext context)
        {
            _context = context;
        }

        public async Task<Setting?> GetSettingsAsync()
        {
            return await _context.Settings.FirstOrDefaultAsync();
        }

        public void Update(Setting setting)
        {
            _context.Settings.Update(setting);
        }
    }
}
