using System.ComponentModel.DataAnnotations;

namespace DTO.Auth
{
    public record RefreshRequestDTO
    {
        [Required]
        public string RefreshToken { get; init; } = null!;

        [Required, EmailAddress]
        public string Email { get; init; } = null!;
    }
}
