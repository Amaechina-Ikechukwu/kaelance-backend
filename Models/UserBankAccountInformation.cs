namespace Kallum.Models
{
    public class UserBankAccountInformation
    {
        public Guid UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
        public Guid BankAccountId {get; set;}

        public BankAccount BankAccount { get; set; }

    }
}
