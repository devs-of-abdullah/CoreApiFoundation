namespace DTO.User
{
    public record ReadUserDTO
    {
        public int Id { get; init; }

        public string Email { get; init; } = null!;

        public string Role { get; init; } = null!;  

        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    
    }
}
