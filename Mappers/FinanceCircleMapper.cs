using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.FinanceCircle;
using Kallum.Models;

namespace Kallum.Mappers
{
    public static class FinanceCircleMapper
    {
        public static FinanceCircle ToCreateFinanceCircleDto(this CreateFinanceCircleDto dto)
        {
            return new FinanceCircle
            {
                CircleId = dto.CircleId,
                Name = dto.Name,
                Friends = dto.Friends,
                FundWithdrawalApprovalCount = dto.FundWithdrawalApprovalCount,
                WithdrawalChargePercentage = dto.WithdrawalChargePercentage,
                PersonalCommittmentPercentage = dto.PersonalCommittmentPercentage,
                CreatorId = dto.CreatorId,

                CircleType = (Models.CircleType)dto.CircleType,
                Status = (Models.Status)dto.Status
            };
        }
    }

}