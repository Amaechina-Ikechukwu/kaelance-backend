using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.FinanceCircle;
using Kallum.Models;

namespace Kallum.Interfaces
{
    public interface IFinanceCircleRepository
    {
        public Task<string> CreateFinanceCircle(CreateFinanceCircleDto circleInfo);

        public Task<List<GetFInanceCircle>> AllFinanceCircle(string username);
    }
}