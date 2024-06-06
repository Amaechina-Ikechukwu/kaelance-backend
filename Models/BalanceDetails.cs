namespace Kallum.Models
{
    public class BalanceDetails
    {
        public Guid BankAccountId { get; set; }
        public UserBankAccountInformation BankAccount { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
