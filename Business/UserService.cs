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

        public async Task<int> RegisterAsync(RegisterUserDto userDto)
        {
            if (await _repo.ExistsByEmailAsync(userDto.Email))
                throw new InvalidOperationException($"'{userDto.Email}' email already exists");


            var user = new UserEntity
            {
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            return await _repo.AddAsync(user);


        }

        public async Task<string> LoginAsync(LoginUserDto userDto)
        {
            var user = await _repo.GetByEmailAsync(userDto.Email)
                ?? throw new UnauthorizedAccessException($"{userDto.Email} is not exists");

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                throw new InvalidOperationException("Wrong password");

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
