namespace Kallum.Models
{
    public class TransactionHistory
    {
        public int Id { get; set; }
        public Guid TransactionHistoryId { get; set; }
        public string? SenderId { get; set; }
        public string? RecieverId { get; set; }

        public string? TransactionDescription { get; set; }

        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
