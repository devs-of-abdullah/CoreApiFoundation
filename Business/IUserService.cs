using Entities.DTOs;

namespace Business
{
    public interface IUserService
    {
        Task<int> RegisterAsync(RegisterUserDto userDto);
        Task<string> LoginAsync(LoginUserDto userDto);

        Task DeleteAsync(int id);
        Task UpdateAsync(int Id,UpdateUserDto userDto);
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<List<UserDto>> GetAllAsync();
    }
}
