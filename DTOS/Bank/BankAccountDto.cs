using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS
{
    public class BankAccountDto
    {

        public string BankAccountId { get; set; }
        public string AccountType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}