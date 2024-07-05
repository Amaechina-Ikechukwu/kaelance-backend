using System.ComponentModel.DataAnnotations;

namespace Kallum.DTOS
{
    public class LoginDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? PassWord { get; set; }
    }
}
