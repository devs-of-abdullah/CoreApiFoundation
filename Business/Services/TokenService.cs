using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DTO.Auth;
using Entities;
namespace Business
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public TokenResponseDTO GenerateTokens(UserEntity user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("User email is required.");

            if (string.IsNullOrWhiteSpace(user.Role))
                throw new InvalidOperationException("User role is required.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var keyString = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int accessTokenMinutes = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var min) ? min : 30;

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateSecureRefreshToken();

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        static string GenerateSecureRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}

