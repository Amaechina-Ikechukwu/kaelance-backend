namespace Kallum.Models
{
    public class BalanceDetails
    {
        public int Id { get; set; }

        public BankAccount? BankAccountDetails { get; set; }
        public double? CurrentBalance { get; set; }
        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public DateTime LastUpdated { get; set; }
        public double TotalCommittment { get; set; }
        public double? DeclinmentCount { get; set; }
    }
}
