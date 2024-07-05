using Kallum.DTOS;

namespace Kallum.Models
{
    public class BankAccount
    {
        public string? Id { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? BankAccountId { get; set; }
        public string? AccountType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
    }
}
