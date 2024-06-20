namespace Kallum.Models
{
    public class UserBankAccountInformation
    {
        public int Id { get; set; }
        public UserAccount UserAccount { get; set; }
        public BankAccount BankAccount { get; set; }

    }
}
