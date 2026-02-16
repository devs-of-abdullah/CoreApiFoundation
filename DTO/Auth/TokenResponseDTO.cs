namespace DTO.Auth
{
    public record TokenResponseDTO
    {
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
    }
}
