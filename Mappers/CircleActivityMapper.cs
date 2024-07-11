using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.CircleDto;
using Kallum.Models;

namespace Kallum.Mappers
{
    public static class CircleActivityMapper
    {
        public static Circle ToCreateCircleActivityDto(this CreateCircleActivity activity)
        {
            return new Circle
            {
                Activity = new Models.Activity
                {
                    ActivityId = activity.Activity.ActivityId,
                    BankId = activity.Activity.BankId,
                    DateTime = activity.Activity.DateTime,
                    ActivityType = (Models.ActivityType)activity.Activity.ActivityType,

                },
                CircleId = activity.CircleId,
                CommitmentHistory = new Models.CommitmentHistory
                {
                    DateTime = activity.CommitmentHistory.DateTime,
                    TransactionId = activity.CommitmentHistory.TransactionId,
                    Percentage = activity.CommitmentHistory.Percentage
                },

            };
        }
        public static CreateCircleActivity ToReturnCircleActivity(this Circle activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");
            }

            return new CreateCircleActivity
            {
                Id = activity.Id,

                CircleId = activity.CircleId,
                CommitmentHistory = activity.CommitmentHistory != null ? new DTOS.CircleDto.CommitmentHistory
                {
                    DateTime = activity.CommitmentHistory.DateTime,
                    TransactionId = activity.CommitmentHistory.TransactionId,
                    Percentage = activity.CommitmentHistory.Percentage
                } : null,
                Activity = activity.Activity != null ? new DTOS.CircleDto.Activity
                {
                    ActivityId = activity.Activity.ActivityId,
                    BankId = activity.Activity.BankId,
                    DateTime = activity.Activity.DateTime,
                    ActivityType = (DTOS.CircleDto.ActivityType)activity.Activity.ActivityType
                } : null,
                WithdrawalApprovalPercentage = activity.WithdrawalApprovalPercentage,
                WithdrawalAction = activity.WithdrawalAction != null ? new DTOS.CircleDto.WithdrawalAction
                {
                    DateTime = activity.WithdrawalAction.DateTime,
                    ApprovalByAll = activity.WithdrawalAction.ApprovalByAll,
                    ApprovedByCreator = activity.WithdrawalAction.ApprovedByCreator,
                    WithdrawalType = (DTOS.CircleDto.WithdrawalType)activity.WithdrawalAction.WithdrawalType
                } : null
            };
        }
    }
}