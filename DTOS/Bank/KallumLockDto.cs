using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.Bank
{
    public class KallumLockDto
    {
        public int SecurePin { get; set; }
        public int TransactionPin { get; set; }
    }
}