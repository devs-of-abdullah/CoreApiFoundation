namespace DTO.Auth
{
    public class RefreshRequestDTO
    {
        public string RefreshToken { get; set; } = null!;
        public string Email { get; set; } = null!;

    }
}
