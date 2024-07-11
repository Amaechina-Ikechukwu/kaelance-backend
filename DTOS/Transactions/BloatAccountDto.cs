using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.Transactions
{
    public class BloatAccountDto
    {
        [Required]
        public string? RecieverId { get; set; }
        [Required]

        public double Amount { get; set; }

    }
}