namespace Kallum.Models
{
    public class UserAccount
    {
        public string? Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PassWord { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
