using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Models
{
    public class KallumLock
    {
        public int Id { get; set; }
        public int SecurePin { get; set; }
        public int TransactionPin { get; set; }
        public string UserAccountId { get; set; }

    }
}