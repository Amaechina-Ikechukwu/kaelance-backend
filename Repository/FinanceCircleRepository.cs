using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.FinanceCircle;
using Kallum.Helper;
using Kallum.Interfaces;
using Kallum.Mappers;
using Kallum.Models;
using Kallum.Service;
using Microsoft.EntityFrameworkCore;
using Sprache;

namespace Kallum.Repository
{
    public class FinanceCircleRepository : IFinanceCircleRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public readonly ServiceComputations _serviceComputations;
        public readonly IBankOperationRepository _bankOperationRepository;
        public FinanceCircleRepository(ApplicationDBContext context, UserIdService userIdService, ServiceComputations serviceComputations, IBankOperationRepository bankOperationRepository)
        {
            _context = context;
            _userIdService = userIdService;
            _serviceComputations = serviceComputations;
            _bankOperationRepository = bankOperationRepository;

        }

        public async Task<List<GetFInanceCircle>> AllFinanceCircle(string username)
        {
            var userId = await _userIdService.GetUserId(username);
            if (userId == null)
            {
                // Log the error
                Console.WriteLine($"User ID not found for username: {username}");
                return null;
            }

            var bankAccountId = await _userIdService.GetBankAccountNumber(userId);
            if (bankAccountId == null)
            {
                // Log the error
                Console.WriteLine($"Bank account ID not found for user ID: {userId}");
                return null;
            }

            var financeCircles = await _context.FinanceCircleData
                .Where(circle => circle.Friends.Contains(bankAccountId) || circle.CreatorId == bankAccountId)
                .ToListAsync();

            var circleList = new List<GetFInanceCircle>();

            foreach (var circle in financeCircles)
            {
                var friendInfos = new List<FriendInformation>();

                foreach (var bankId in circle.Friends)
                {
                    try
                    {
                        var accountInfo = await _userIdService.GetBankAccountInfo(bankId);
                        if (accountInfo == null)
                        {

                            continue;
                        }

                        var friendInfo = new FriendInformation
                        {
                            UserName = accountInfo.KallumUser.UserName,
                            Email = accountInfo.KallumUser.Email
                        };
                        friendInfos.Add(friendInfo);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception
                        Console.WriteLine($"Exception occurred while getting bank account info for bank ID: {bankId}. Exception: {ex.Message}");
                    }
                }

                var transactionHistory = circle.TransactionHistory?.Select(t => new Transaction
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    Date = t.Date,
                    Description = t.Description
                }).ToList() ?? new List<Transaction>();

                var circleInfo = new GetFInanceCircle
                {
                    CircleId = circle.CircleId,
                    Name = circle.Name,
                    TotalAmountCommitted = circle.TotalAmountCommitted,
                    Friends = friendInfos.Take(6).ToList(),
                    FundWithdrawalApprovalCount = circle.FundWithdrawalApprovalCount,
                    WithdrawalChargePercentage = circle.WithdrawalChargePercentage,
                    PersonalCommittmentPercentage = circle.PersonalCommittmentPercentage,
                    CreatorId = circle.CreatorId,
                    TransactionHistory = transactionHistory,
                    CircleType = (DTOS.FinanceCircle.CircleType)circle.CircleType,
                    WithdrawalLimitPercentage = circle.WithdrawalLimitPercentage
                };

                circleList.Add(circleInfo);
            }

            return circleList;
        }

        public async Task<string> CreateFinanceCircle(CreateFinanceCircleDto circleInfo, string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                bool isEligible = await _serviceComputations.UpdateUsersCommittments(circleInfo.CreatorId, circleInfo.PersonalCommittmentPercentage);
                if (isEligible)
                {
                    circleInfo.CircleId = Guid.NewGuid();

                    circleInfo.CreatorId = userId;
                    await _context.FinanceCircleData.AddAsync(circleInfo.ToCreateFinanceCircleDto());
                    await _context.SaveChangesAsync();
                    return "Group Created";
                }
                else
                {
                    return "You do not have enough fund to commit";
                }




            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<EligibilityResult> IsUserEligible(string username)
        {

            var balanceInfo = await _context.BalanceDetailsData
                .Include(b => b.BankAccountDetails)
                .FirstOrDefaultAsync(b => b.BankAccountDetails.AppUser.UserName == username);

            if (balanceInfo == null)
            {
                return new EligibilityResult { Result = false, Message = "User balnce info no found" }; // Balance information not found
            }
            if (balanceInfo.TotalCommittment == 0 && balanceInfo.CurrentBalance > 10)
            {
                return new EligibilityResult { Result = true };
            }
            decimal totalCommittmentValue = (decimal)((decimal)balanceInfo.TotalCommittment / 100 * balanceInfo.CurrentBalance);
            Console.WriteLine($"TotalCommitmentValue: {totalCommittmentValue}");
            if (totalCommittmentValue > 5)
            {
                return new EligibilityResult { Result = true };
            }
            else
            {
                return new EligibilityResult { Result = false, Message = "Your current balance may not support this new circle. Please top up" };
            }

        }
    }
}