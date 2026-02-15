namespace DTO.Auth
{
    public class LogoutRequestDTO
    {
        public string Email { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
