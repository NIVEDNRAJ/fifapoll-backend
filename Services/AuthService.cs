using System;
using System.Threading.Tasks;
using AutoMapper;
using FifaPollApi.Common;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;
using FifaPollApi.DTOs.Auth;
using FifaPollApi.Security;

namespace FifaPollApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await _unitOfWork.Users.ExistsByEmailAsync(registerDto.Email))
            {
                return ApiResponse<AuthResponseDto>.Fail("A user with this email address already exists.");
            }

            var passwordHash = _passwordHasher.HashPassword(registerDto.Password);
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                Role = "User", // Default registered role is User
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var token = _jwtTokenGenerator.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return ApiResponse<AuthResponseDto>.Ok(response, "Registration successful!");
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email.ToLowerInvariant());
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return ApiResponse<AuthResponseDto>.Ok(response, "Login successful!");
        }
    }
}
