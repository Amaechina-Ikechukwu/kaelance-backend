using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Models
{
    public class Circle
    {
        public int Id { get; set; }
        public required string CircleId { get; set; }
        public required CommitmentHistory CommitmentHistory { get; set; }
        public required Activity Activity { get; set; }
        public int WithdrawalApprovalPercentage { get; set; }

    }




    // Helper class for transactions
    public class CommitmentHistory
    {
        // Unique identifier for the transaction
        public required Guid TransactionId { get; set; }

        // Date of the transaction
        public DateTime Date { get; set; }

        // Amount of the transaction
        public int Percentage { get; set; }


        public required DateTime DateTime { get; set; }
    }
    public class Activity
    {
        // Unique identifier for the transaction
        public required Guid ActivityId { get; set; }

        // Date of the transaction
        public DateTime Date { get; set; }


        public required string BankId { get; set; }


        public ActivityType ActivityType { get; set; }

        public required DateTime DateTime { get; set; }
    }
    public enum ActivityType
    {
        Commitmment, Declinement,
    }

}