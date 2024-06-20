using Kallum.DTOS;

namespace Kallum.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        public string UserAccountId { get; set; }
        public string BankAccountId { get; set; }
        public string AccountType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
