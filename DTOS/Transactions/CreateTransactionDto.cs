using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.Transactions
{
    public class CreateTransactionDto
    {
        public string? SenderId { get; set; }
        public string? RecieverId { get; set; }

        public string? TransactionDescription { get; set; }


        public double Amount { get; set; }
    }
}