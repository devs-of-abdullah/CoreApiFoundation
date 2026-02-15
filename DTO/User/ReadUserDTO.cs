namespace DTO.User
{
    public class ReadUserDTO
    {
        public int Id { get; set; } 
        public string? Email { get; set; } 
        public string? Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }
    }
}
