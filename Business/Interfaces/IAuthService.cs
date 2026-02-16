using DTO.Auth;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO?> Login(LoginRequestDTO dto);
        Task<TokenResponseDTO?> RefreshToken(RefreshRequestDTO dto);
        Task Logout(int id, string refreshToken);
    }
}
