using System.Threading.Tasks;
using AutoMapper;
using FifaPollApi.Common;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;
using FifaPollApi.DTOs.Setting;

namespace FifaPollApi.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SettingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SettingDto>> GetSettingsAsync()
        {
            var settings = await _unitOfWork.Settings.GetSettingsAsync();
            if (settings == null)
            {
                // Fallback (though it should be seeded)
                var defaultSettings = new SettingDto { IsVotingEnabled = true, IsResultPublished = false };
                return ApiResponse<SettingDto>.Ok(defaultSettings);
            }

            var dto = _mapper.Map<SettingDto>(settings);
            return ApiResponse<SettingDto>.Ok(dto);
        }

        public async Task<ApiResponse<SettingDto>> UpdateSettingsAsync(SettingDto settingDto)
        {
            var settings = await _unitOfWork.Settings.GetSettingsAsync();
            if (settings == null)
            {
                // Create if it doesn't exist
                settings = new Setting();
                // We'd need to add it, but since we assume it's seeded:
                return ApiResponse<SettingDto>.Fail("Settings database record not found.");
            }

            // Map values
            settings.IsVotingEnabled = settingDto.IsVotingEnabled;
            settings.IsResultPublished = settingDto.IsResultPublished;

            _unitOfWork.Settings.Update(settings);
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<SettingDto>(settings);
            return ApiResponse<SettingDto>.Ok(dto, "Settings updated successfully!");
        }
    }
}
