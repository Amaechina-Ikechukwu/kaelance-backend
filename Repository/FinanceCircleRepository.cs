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
            var bankAccountId = await _userIdService.GetBankAccountNumber(userId);
            if (userId == null)
            {
                return null;
            }

            var financeCircles = await _context.FinanceCircleData
                .Where(circle => circle.Friends.Any(friend => friend.BankId == bankAccountId))
                .ToListAsync();

            var circleList = new List<GetFInanceCircle>();

            foreach (var circle in financeCircles)
            {
                var friendInfos = new List<FriendInformation>();

                foreach (var friend in circle.Friends)
                {
                    var accountInfo = await _userIdService.GetBankAccountInfo(friend.BankId);
                    var friendInfo = new FriendInformation
                    {
                        UserName = accountInfo.KallumUser.UserName,
                        Email = accountInfo.KallumUser.Email
                    };
                    friendInfos.Add(friendInfo);
                }
                var transactionHistory = circle.TransactionHistory.Select(t => new Transaction
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    Date = t.Date,
                    Description = t.Description

                    // Map other properties as needed
                }).ToList();
                var circleInfo = new GetFInanceCircle
                {
                    CircleId = circle.CircleId,
                    Name = circle.Name,
                    TotalAmountCommitted = circle.TotalAmountCommitted,
                    Friends = friendInfos.Take(6).ToList(),
                    FundWithdrawalApprovalCount = circle.FundWithdrawalApprovalCount,
                    WithdrawalStatus = circle.WithdrawalStatus,
                    WithdrawalInitiatorId = circle.WithdrawalInitiatorId,
                    WithdrawalLimitPercentage = circle.WithdrawalLimitPercentage,
                    CreatorId = circle.CreatorId,
                    TransactionHistory = transactionHistory,
                    CircleType = (DTOS.FinanceCircle.CircleType)circle.CircleType
                };

                circleList.Add(circleInfo);
            }

            return circleList;
        }



        public async Task<string> CreateFinanceCircle(CreateFinanceCircleDto circleInfo)
        {
            try
            {

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