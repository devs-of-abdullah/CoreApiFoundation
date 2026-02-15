using Business.Interfaces;
using Data;
using DTO.User;
using Entities;
namespace Business.Services
{
    public class UserService : IUserService
    {
        readonly IUserRepository _repo;
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> CreateAsync(CreateUserDTO userDto)
        {
            if (await _repo.ExistsByEmailAsync(userDto.Email))
                throw new InvalidOperationException($"'{userDto.Email}' email already exists");


            var user = new UserEntity
            {
                Email = userDto.Email,
                Role = userDto.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            return await _repo.CreateAsync(user);


        }

        public async Task<ReadUserDTO?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;


            return new ReadUserDTO
            {
                Id = user.Id,
                Role = user.Role,
                Email = user.Email,
            };
        }
        public async Task<ReadUserDTO?> GetByEmailAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            return new ReadUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
        public async Task SoftDeleteAsync(int Id, string currentPassword)
        {
            var user = await _repo.GetByIdAsync(Id);

            if (user == null)
                throw new KeyNotFoundException("User not found");
           
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");
            
            if (user.IsDeleted)
                throw new InvalidOperationException("User already deleted");
           
            user.IsDeleted = true;

            await _repo.UpdateAsync(user);
        }
      
        public async Task UpdatePasswordAsync(int id, string currentPassword, string newPassword)
        {
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");

            if (BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
                throw new Exception("New password cannot be same as old password");
           
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _repo.UpdateAsync(user);

        }
        public async Task UpdateRoleAsync(int id,string currentUserRole, string newRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only admins can change roles");
           
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.Role = newRole;
            await _repo.UpdateAsync(user);

        }
        public async Task UpdateEmailAsync(int id, string newEmail, string currentPassword)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email cannot be empty");
           
            newEmail = newEmail.Trim().ToLower();

            var user = await _repo.GetByIdAsync(id);

            if (user == null)
               throw new KeyNotFoundException("User not found");
            
            if (user.IsDeleted)
                throw new InvalidOperationException("User account is deleted");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Password is incorrect");
            
            if (user.Email.ToLower() == newEmail)
                throw new InvalidOperationException("New email cannot be same as current email");

            var existing = await _repo.GetByEmailAsync(newEmail);
            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Email already in use");
            
            user.Email = newEmail;
           
            await _repo.UpdateAsync(user);


        }
    }

}