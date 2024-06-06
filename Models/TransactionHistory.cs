namespace Kallum.Models
{
    public class TransactionHistory
    {
        public Guid TransactionHistoryId { get; set; }
        public Guid BankAccountId { get; set; }

        public string TransactionDescription { get; set; }
        public string TransactionType { get; set; }


        public Decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
