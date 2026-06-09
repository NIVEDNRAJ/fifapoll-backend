using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Setting;
using FifaPollApi.Services;

namespace FifaPollApi.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<SettingDto>>> Get()
        {
            var response = await _settingService.GetSettingsAsync();
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<SettingDto>>> Update([FromBody] SettingDto settingDto)
        {
            var response = await _settingService.UpdateSettingsAsync(settingDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
