using DTO.Auth;
using Data;
using Microsoft.Extensions.Configuration;
using Business.Interfaces;
namespace Business.Services
{
    public class AuthService : IAuthService
    {
         readonly IUserRepository _userRepository;
         readonly ITokenService _tokenService;
         readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IConfiguration config)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _config = config;
        }

        public async Task<TokenResponseDTO?> Login(LoginRequestDTO request)
        {
            var email = request.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || user.IsDeleted)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var tokens = _tokenService.GenerateToken(user);

            int refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "7");

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(tokens.RefreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshDays);
            user.RefreshTokenRevokedAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return tokens;
        }

        public async Task<TokenResponseDTO?> RefreshToken(RefreshRequestDTO request)
        {
            var email = request.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || user.IsDeleted)
                return null;

            if (user.RefreshTokenRevokedAt != null)
                return null;

            if (user.RefreshTokenExpiresAt == null ||
                user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return null;

            if (string.IsNullOrEmpty(user.RefreshTokenHash) ||
                !BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash))
                return null;

            var tokens = _tokenService.GenerateToken(user);

            int refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "7");

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(tokens.RefreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshDays);
            user.RefreshTokenRevokedAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return tokens;
        }

        public async Task Logout(int userId, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
                return;

            if (user.RefreshTokenHash != null &&
                BCrypt.Net.BCrypt.Verify(refreshToken, user.RefreshTokenHash))
            {
                user.RefreshTokenRevokedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}

