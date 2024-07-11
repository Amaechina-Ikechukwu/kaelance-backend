using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.FinanceCircle
{
    public class CreateFinanceCircleDto
    {
        // Unique identifier for the finance circle
        public Guid CircleId { get; set; }

        // Name of the circle
        public required string Name { get; set; }



        // List of friends with name and email (limit to six)
        public List<string> Friends { get; set; } = new List<string>();
        public double TargetAmount { get; set; }
        // Fund withdrawal approval count
        public double FundWithdrawalApprovalCount { get; set; }
        // Fund withdrawal approval count

        public double TotalCommittment { get; set; }
        public double PersonalCommittmentPercentage { get; set; }

        public double WithdrawalChargePercentage { get; set; }

        // ID of the creator of the circle
        public required string CreatorId { get; set; }



        // Type of circle (monarch or democratic)
        public CircleType CircleType { get; set; }
        public Status Status { get; set; }
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

    public enum Status
    {
        Active, Suspended, Disabled, Deleted
    }
}