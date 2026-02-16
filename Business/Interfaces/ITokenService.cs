

using DTO.Auth;
using Entities;

namespace Business.Interfaces
{
    public interface ITokenService
    {
        TokenResponseDTO GenerateToken(UserEntity entity);
    }
}
