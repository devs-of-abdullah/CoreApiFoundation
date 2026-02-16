
using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    public record CreateUserDTO
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; init; } = null!;

        [Required, MinLength(6)]
        public string Password { get; init; } = null!;

        [Required]
        public string Role { get; init; } = null!;

       
    }
}
