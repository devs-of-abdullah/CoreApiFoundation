using DTO.Auth;
using Data;

namespace Business.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthService(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseDTO?> Login(LoginRequestDTO request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var tokens = _tokenService.GenerateTokens(user);

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(tokens.RefreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.RefreshTokenRevokedAt = null;

            await _userRepository.UpdateAsync(user);

            return tokens;
        }

        public async Task<TokenResponseDTO?> RefreshToken(RefreshRequestDTO request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return null;

            if (user.RefreshTokenRevokedAt != null)
                return null;

            if (user.RefreshTokenExpiresAt == null ||
                user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash))
                return null;

            // Token rotation
            var tokens = _tokenService.GenerateTokens(user);

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(tokens.RefreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.RefreshTokenRevokedAt = null;

            await _userRepository.UpdateAsync(user);

            return tokens;
        }

        public async Task Logout(LogoutRequestDTO request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return;

            if (user.RefreshTokenHash != null &&
                BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash))
            {
                user.RefreshTokenRevokedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
