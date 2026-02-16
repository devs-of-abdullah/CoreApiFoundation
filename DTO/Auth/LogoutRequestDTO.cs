using System.ComponentModel.DataAnnotations;

namespace DTO.Auth
{
    public record LogoutRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        public string RefreshToken { get; init; } = null!;
    }
}
