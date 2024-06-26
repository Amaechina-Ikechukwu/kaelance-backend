using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.FinanceCircle
{
    public class CreateFinanceCircleDto
    {
        // Unique identifier for the finance circle
        public required string CircleId { get; set; }

        // Name of the circle
        public required string Name { get; set; }

        // Total amount committed to this circle by friends
        public decimal TotalAmountCommitted { get; set; }

        // List of friends with name and email (limit to six)
        public List<Friend> Friends { get; set; } = new List<Friend>();

        // Fund withdrawal approval count
        public int FundWithdrawalApprovalCount { get; set; }

        // Withdrawal status
        public required string WithdrawalStatus { get; set; }

        // ID of who initiated the withdrawal
        public required string WithdrawalInitiatorId { get; set; }

        //Withdrawl percentage limit {get;set;}

        public required int WithdrawalLimitPercentage { get; set; }

        // ID of the creator of the circle
        public required string CreatorId { get; set; }



        // Type of circle (monarch or democratic)
        public CircleType CircleType { get; set; }
    }
    // Helper class for friends
    public class Friend
    {
        public required string BankId { get; set; }
    }

    // Helper class for transactions


    // Enum for circle type
    public enum CircleType
    {
        Monarch,
        Democratic
    }
}