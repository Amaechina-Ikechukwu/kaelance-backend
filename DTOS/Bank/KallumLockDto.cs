using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.Bank
{
    public class KallumLockDto
    {
        public string? SecurePin { get; set; }


        public string? TransactionPin { get; set; }

    }

}
