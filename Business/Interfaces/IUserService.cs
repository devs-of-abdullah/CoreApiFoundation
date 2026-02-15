using DTO.User;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateAsync(CreateUserDTO UserDto);
        Task SoftDeleteAsync(int id);
        Task UpdateAsync(int Id,UpdateUserDTO userDto);
        Task<ReadUserDTO?> GetByIdAsync(int id);
        Task<ReadUserDTO?> GetByEmailAsync(string email);
    }
}
