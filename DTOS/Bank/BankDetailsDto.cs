using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Models;

namespace Kallum.DTOS
{
    public class BankDetailsDto
    {

        public string UserAccountId { get; set; }
        public string BankAccountId { get; set; }
        public UserBankAccountInformation BankAccount { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}