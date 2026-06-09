using System.Threading.Tasks;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Setting;

namespace FifaPollApi.Services
{
    public interface ISettingService
    {
        Task<ApiResponse<SettingDto>> GetSettingsAsync();
        Task<ApiResponse<SettingDto>> UpdateSettingsAsync(SettingDto settingDto);
    }
}
