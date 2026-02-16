

using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    public record SoftUserDeleteDTO
    {

        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; init; } = null!;
    }
}
