using System.ComponentModel.DataAnnotations;

namespace DTO.Auth
{
    public record LoginRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        public string Password { get; init; } = null!;
    }
}
