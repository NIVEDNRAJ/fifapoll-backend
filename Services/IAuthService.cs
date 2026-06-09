using System.Threading.Tasks;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Auth;

namespace FifaPollApi.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    }
}
