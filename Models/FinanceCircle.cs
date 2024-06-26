using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Models
{
    public class FinanceCircle
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

        // Circle transaction history
        public List<Transaction> TransactionHistory { get; set; } = new List<Transaction>();

        // Type of circle (monarch or democratic)
        public CircleType CircleType { get; set; }
    }

    // Helper class for friends
    public class Friend
    {
        // Name of the friend
        public required string BankId { get; set; }


    }

    // Helper class for transactions
    public class Transaction
    {
        // Unique identifier for the transaction
        public required Guid TransactionId { get; set; }

        // Date of the transaction
        public DateTime Date { get; set; }

        // Amount of the transaction
        public decimal Amount { get; set; }

        // Description of the transaction
        public required string Description { get; set; }
    }

    // Enum for circle type
    public enum CircleType
    {
        Monarch,
        Democratic
    }
}