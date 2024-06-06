namespace Kallum.Models
{
    public class BankAccount
    {
        public Guid BankAccountId { get; set; }
        public string AccountType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public List<UserBankAccountInformation> UserBankAccounts { get; set; }
    }
}
