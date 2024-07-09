using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.FinanceCircle;
using Kallum.Helper;
using Kallum.Models;

namespace Kallum.Interfaces
{
    public interface IFinanceCircleRepository
    {
        public Task<CreateCircleResponseDto?> CreateFinanceCircle(CreateFinanceCircleDto circleInfo, string username);

        public Task<List<GetFInanceCircle?>> AllFinanceCircle(string username);

        public Task<EligibilityResult> IsUserEligible(string username);
    }
}