using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Models;

namespace Kallum.DTOS.FinanceCircle
{
    public class GetFInanceCircle
    {
        public required string CircleId { get; set; }

        // Name of the circle
        public required string Name { get; set; }

        // Total amount committed to this circle by friends
        public decimal TotalAmountCommitted { get; set; }

        // List of friends with name and email (limit to six)
        public List<FriendInformation> Friends { get; set; } = new List<FriendInformation>();

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

        // Circle transaction history
        public List<Transaction> TransactionHistory { get; set; } = new List<Transaction>();

        // Type of circle (monarch or democratic)
        public CircleType CircleType { get; set; }


    }
    // Helper class for friends
    public class FriendInformation
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}