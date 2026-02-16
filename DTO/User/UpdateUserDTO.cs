

using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    public record UpdateUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        public string Role { get; init; } = null!;  
    }
}
