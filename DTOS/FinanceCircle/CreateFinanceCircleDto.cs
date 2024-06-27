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

        // Fund withdrawal approval count
        public int FundWithdrawalApprovalCount { get; set; }
        // Fund withdrawal approval count


        public int PersonalCommittmentPercentage { get; set; }

        public required int WithdrawalChargePercentage { get; set; }

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