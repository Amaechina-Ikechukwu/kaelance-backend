using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Models
{
    public class Circle
    {
        public int Id { get; set; }
        public required Guid CircleId { get; set; }
        public CommitmentHistory? CommitmentHistory { get; set; }
        public required Activity Activity { get; set; }
        public double WithdrawalApprovalPercentage { get; set; }

        public WithdrawalAction? WithdrawalAction { get; set; }

    }



    public class WithdrawalAction
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; }
        public bool ApprovedByCreator { get; set; }
        public double ApprovalByAll { get; set; }
        public WithdrawalType WithdrawalType
        {
            get; set;
        }
    }

    public enum WithdrawalType
    {
        PersonalWithdrawal, WithdrawalAll
    }
    // Helper class for transactions
    public class CommitmentHistory
    {

        public int Id { get; set; }// Unique identifier for the transaction
        public required Guid TransactionId { get; set; }



        // Amount of the transaction
        public double Percentage { get; set; }


        public required DateTime DateTime { get; set; }
    }
    public class Activity
    {
        public int Id { get; set; }
        // Unique identifier for the transaction
        public required Guid ActivityId { get; set; }




        public required string BankId { get; set; }


        public ActivityType ActivityType { get; set; }

        public required DateTime DateTime { get; set; }

    }
    public enum ActivityType
    {
        Commitmment, Declinement,
    }

}