using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Models
{
    public class KallumLock
    {
        public int Id { get; set; }
        public string? SecurePin { get; set; }
        public string? TransactionPin { get; set; }
        public required string AppUserId { get; set; }

    }
}