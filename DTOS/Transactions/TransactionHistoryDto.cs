using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Models;

namespace Kallum.DTOS.Transactions
{
    public class TransactionHistoryDto
    {

        public int Id { get; set; }
        public Guid TransactionHistoryId { get; set; }
        public BankAccountDto? Sender { get; set; }
        public BankAccountDto? Reciever { get; set; }

        public string? TransactionDescription { get; set; }
        public string? TransationType { get; set; }

        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

    }
}