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
    public record LogoutRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        public string RefreshToken { get; init; } = null!;
    }
    public record RefreshRequestDTO
    {
        [Required]
        public string RefreshToken { get; init; } = null!;

        [Required, EmailAddress]
        public string Email { get; init; } = null!;
    }
    public record TokenResponseDTO
    {
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
    }
}
