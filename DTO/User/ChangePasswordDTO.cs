
using System.ComponentModel.DataAnnotations;

namespace DTO.User
{   
       public record ChangePasswordDTO
        {
            [Required]
            public string CurrentPassword { get; init; } = null!;

            [Required, MinLength(6)]
            public string NewPassword { get; init; } = null!;
        }
    }
