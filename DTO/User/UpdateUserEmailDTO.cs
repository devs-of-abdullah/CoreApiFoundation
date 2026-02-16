

using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    public record UpdateUserEmailDTO
    {
        [Required] 
        public int Id { get; init; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string NewEmail { get; init; } = null!;

        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; init; } = null!;


    }
}
