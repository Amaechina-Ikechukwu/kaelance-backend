using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Helper
{
    public class CircleResponseDto
    {
        public required string Message { get; set; }
        public Guid? CircleId { get; set; }
    }
}