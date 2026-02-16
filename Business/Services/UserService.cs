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
        public async Task<int> CreateAsync(CreateUserDTO dto)
        {
            if (await _repo.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException($"'{dto.Email}' email already exists");


            var user = new UserEntity
            {
                Email = dto.Email,
                Role = dto.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
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
        public async Task SoftDeleteAsync(int Id, SoftUserDeleteDTO dto)
        {
            var user = await _repo.GetByIdAsync(Id);

            if (user == null)
                throw new KeyNotFoundException("User not found");
           
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");
            
            if (user.IsDeleted)
                throw new InvalidOperationException("User already deleted");
           
            user.IsDeleted = true;

            await _repo.UpdateAsync(user);
        }
        public async Task UpdatePasswordAsync(int userId, UpdateUserPasswordDTO dto)
        {
            var user = await _repo.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
                throw new InvalidOperationException("New password cannot be same as old password");

            if (dto.NewPassword.Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, workFactor: 12);

            user.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user);
        }
        public async Task UpdateRoleAsync(int id, UpdateUserRoleDTO dto)
        {         
           
            var user = await _repo.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.Role = dto.NewRole;
            await _repo.UpdateAsync(user);

        }
        public async Task UpdateEmailAsync(int userId, UpdateUserEmailDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewEmail))
                throw new ArgumentException("Email cannot be empty");

            var normalizedEmail = dto.NewEmail.Trim().ToLower();

            var user = await _repo.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (user.IsDeleted)
                throw new InvalidOperationException("User account is deleted");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Password is incorrect");

            if (user.Email == normalizedEmail)
                throw new InvalidOperationException("New email cannot be same as current email");

            var existing = await _repo.GetByEmailAsync(normalizedEmail);

            if (existing != null && existing.Id != userId)
                throw new InvalidOperationException("Email already in use");

            user.Email = normalizedEmail;
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user);
        }

    }

}