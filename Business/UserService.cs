using Data;
using Entities;
using Entities.DTOs;
namespace Business
{
    public class UserService : IUserService
    {
        readonly IUserRepository _repo;
        readonly TokenService _tokenService;
        public UserService(IUserRepository repo, TokenService token)
        {
            _repo = repo;
            _tokenService = token;
        }

        public async Task<int> RegisterAsync(string email, string password)
        {
            if (await _repo.ExistsByEmailAsync(email))
                throw new InvalidOperationException("User already exists");

            var user = new UserEntity
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _repo.AddAsync(user);
            
            return user.Id;

        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email)
                ?? throw new UnauthorizedAccessException("Invalid credintials");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credintials");

            return _tokenService.CreateToken(user);

        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
            }).ToList();
        } 

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;

           
            return new UserDto
            {
                Id=user.Id,
                Email = user.Email,
            };
        }
        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if(user == null) return null;

            return new UserDto
            {
                Id= user.Id,
                Email = user.Email,
            };
        }
        public async Task DeleteAsync(int Id)
        {
          await  _repo.DeleteAsync(Id);
        }
        public async Task UpdateAsync(int Id, UpdateUserDto dto)
        {
            var user = await _repo.GetByIdAsync(Id) 
                ?? throw new KeyNotFoundException("User not found");

            user.Email = dto.Email;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
             
            await _repo.UpdateAsync(user);
        }

    }

}
