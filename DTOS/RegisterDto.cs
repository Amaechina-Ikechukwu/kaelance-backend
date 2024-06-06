using System.ComponentModel.DataAnnotations;

namespace Kallum.DTOS
{
    public class RegisterDto
    {
        [Required]

        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? PassWord { get; set; }
        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
