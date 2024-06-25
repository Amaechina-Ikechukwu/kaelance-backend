using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.Bank
{

    public class TransactionPinRequest
    {
        public required string TransactionPin { get; set; }
    }
    public class SecurePinRequest
    {
        public required string SecurePin { get; set; }
    }

}