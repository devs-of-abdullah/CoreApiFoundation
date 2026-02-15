using Microsoft.AspNetCore.Identity.Data;
using DTO.Auth;
using Business;
using Data;

public class AuthService
{
    readonly IUserRepository _userRepository;
    readonly TokenService _tokenService;

    public AuthService(TokenService tokenService,IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }
    public async Task<TokenResponseDTO?> Login(LoginRequestDTO request)
    {
        var user =  await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || BCrypt.Net.BCrypt.Verify(request.Password, user.Email))
            return null;

        return await _tokenService.CreateToken(request.Email);
    }

    public async Task<TokenResponseDTO?> RefreshToken(RefreshRequestDTO request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if ( user == null
            || user.RefreshTokenRevokedAt != null
            || user.RefreshTokenExpiresAt == null
            || user.RefreshTokenExpiresAt <= DateTime.UtcNow
            || !BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash))
            return null;

        return await _tokenService.CreateToken(request.Email);
    }

    public async Task Logout(LogoutRequestDTO request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            return;

        if (BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash))
        {
            user.RefreshTokenRevokedAt = DateTime.UtcNow;
        }
    }

   

   
}
