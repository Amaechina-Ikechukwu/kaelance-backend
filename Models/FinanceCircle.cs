using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Models
{
    public class FinanceCircle
    {
        public int Id { get; set; }
        // Unique identifier for the finance circle
        public Guid CircleId { get; set; }

        // Name of the circle
        public required string Name { get; set; }

        // Total amount committed to this circle by friends
        public double TotalAmountCommitted { get; set; }

        // List of friends with name and email (limit to six)
        public List<string> Friends { get; set; } = new List<string>();

        // Fund withdrawal approval count
        public double TargetAmount { get; set; }
        public double FundWithdrawalApprovalCount { get; set; }
        public double TotalCommittment { get; set; }
        public double PersonalCommittmentPercentage { get; set; }

        public required double WithdrawalChargePercentage { get; set; }

        // ID of the creator of the circle
        public required string CreatorId { get; set; }

        // Circle transaction history
        public List<Transaction> TransactionHistory { get; set; } = new List<Transaction>();

        // Type of circle (monarch or democratic)
        public CircleType CircleType { get; set; }
        //The activity state of the app
        public Status Status { get; set; }
    }

    // Helper class for friends
    public class Friend
    {
        public int Id { get; set; }
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
        public double Amount { get; set; }

        // Description of the transaction
        public required string Description { get; set; }
    }

    // Enum for circle type
    public enum CircleType
    {
        Monarch,
        Democratic
    }

    public enum Status
    {
        Active, Suspended, Disabled, Deleted
    }
}