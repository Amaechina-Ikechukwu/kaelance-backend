using System;
using System.Collections.Generic;
using System.Linq;
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
                TargetAmount = dto.TargetAmount,
                CircleType = (Models.CircleType)dto.CircleType,
                Status = (Models.Status)dto.Status,
                TotalCommittment = dto.TotalCommittment
            };
        }

        public static GetFInanceCircle ToGetAllFinanceCircleDto(this GetFInanceCircle dto)
        {
            return new GetFInanceCircle
            {
                CircleId = dto.CircleId,
                Name = dto.Name,
                Friends = dto.Friends,
                CreatorId = dto.CreatorId
            };
        }

        public static GetFInanceCircle ToGetFinanceCircleDto(this GetFInanceCircle dto)
        {
            return new GetFInanceCircle
            {
                CircleId = dto.CircleId,
                Name = dto.Name,
                Friends = dto.Friends,
                FundWithdrawalApprovalCount = dto.FundWithdrawalApprovalCount,
                WithdrawalChargePercentage = dto.WithdrawalChargePercentage,
                PersonalCommittmentPercentage = dto.PersonalCommittmentPercentage,
                CreatorId = dto.CreatorId,
                TargetAmount = dto.TargetAmount,
                CircleType = dto.CircleType,
                Status = dto.Status,
                TotalCommittment = dto.TotalCommittment
            };
        }
    }
}
