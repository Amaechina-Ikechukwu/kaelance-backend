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
                TotalAmountCommitted = dto.TotalAmountCommitted,
                Friends = dto.Friends.Select(f => new Kallum.Models.Friend
                {
                    BankId = f.BankId
                }).ToList(),
                FundWithdrawalApprovalCount = dto.FundWithdrawalApprovalCount,
                WithdrawalStatus = dto.WithdrawalStatus,
                WithdrawalInitiatorId = dto.WithdrawalInitiatorId,
                WithdrawalLimitPercentage = dto.WithdrawalLimitPercentage,
                CreatorId = dto.CreatorId,

                CircleType = (Models.CircleType)dto.CircleType
            };
        }
    }

}