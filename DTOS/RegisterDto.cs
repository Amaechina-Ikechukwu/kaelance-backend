using System.ComponentModel.DataAnnotations;

namespace Kallum.DTOS
{
    public class RegisterDto
    {
        [Required]

        public required string FullName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string PassWord { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
    }
}
