using DTO.User;
namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateAsync(CreateUserDTO UserDto);
        Task SoftDeleteAsync(int id, string currentPassword);
        Task<ReadUserDTO?> GetByIdAsync(int id);
        Task<ReadUserDTO?> GetByEmailAsync(string email);
        Task UpdatePasswordAsync(int id, string currentPassword, string newPassword);
        Task UpdateRoleAsync(int id,string currentUserRole, string newRole);
        Task UpdateEmailAsync(int id, string newEmail, string currentPassword);
    }
}
