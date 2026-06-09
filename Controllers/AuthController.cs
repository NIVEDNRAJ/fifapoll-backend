using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Auth;
using FifaPollApi.Services;

namespace FifaPollApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.Fail("Validation failed", errors));
            }

            var response = await _authService.RegisterAsync(registerDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.Fail("Validation failed", errors));
            }

            var response = await _authService.LoginAsync(loginDto);
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}
