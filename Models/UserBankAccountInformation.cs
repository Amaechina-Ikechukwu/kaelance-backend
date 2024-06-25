namespace Kallum.Models
{
    public class UserBankAccountInformation
    {
        public string AppUserId { get; set; }
        // public required string AppUserId { get; set; } // This should be a property of the entity

        public required AppUser AppUser { get; set; }
        public required BankAccount BankAccount { get; set; }
        public TransactionHistory TransactionHistory { get; set; }

    }
}
