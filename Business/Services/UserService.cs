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
        public async Task SoftDeleteAsync(int Id)
        {
            await _repo.SoftDeleteAsync(Id);
        }
        public async Task UpdateAsync(int Id, UpdateUserDTO dto)
        {
            var user = await _repo.GetByIdAsync(Id)
                ?? throw new KeyNotFoundException("User not found");

            user.Role = dto.Role;
            user.Email = dto.Email;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repo.UpdateAsync(user);
        }

    }

}