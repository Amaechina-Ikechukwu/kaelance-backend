using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.FinanceCircle;
using Kallum.Interfaces;
using Kallum.Mappers;
using Kallum.Models;
using Kallum.Service;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Repository
{
    public class FinanceCircleRepository : IFinanceCircleRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public FinanceCircleRepository(ApplicationDBContext context, UserIdService userIdService)
        {
            _context = context;
            _userIdService = userIdService;
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
                            // Log the error
                            Console.WriteLine($"Account info not found for bank ID: {bankId}");
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

        public async Task<string> CreateFinanceCircle(CreateFinanceCircleDto circleInfo)
        {
            try
            {
                circleInfo.CircleId = Guid.NewGuid();
                Console.WriteLine(circleInfo.CircleId);

                await _context.FinanceCircleData.AddAsync(circleInfo.ToCreateFinanceCircleDto());
                await _context.SaveChangesAsync();
                return "Group Created";

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }


    }
}