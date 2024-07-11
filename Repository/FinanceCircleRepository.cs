using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.CircleDto;
using Kallum.DTOS.FinanceCircle;
using Kallum.Helper;
using Kallum.Interfaces;
using Kallum.Mappers;
using Kallum.Models;
using Kallum.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sprache;

namespace Kallum.Repository
{
    public class FinanceCircleRepository : IFinanceCircleRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public readonly ServiceComputations _serviceComputations;
        public readonly IBankOperationRepository _bankOperationRepository;
        public readonly ICircleRepository _circleRepository;

        public FinanceCircleRepository(ApplicationDBContext context, UserIdService userIdService, ServiceComputations serviceComputations, IBankOperationRepository bankOperationRepository, ICircleRepository circleRepository)
        {
            _context = context;
            _userIdService = userIdService;
            _serviceComputations = serviceComputations;
            _bankOperationRepository = bankOperationRepository;
            _circleRepository = circleRepository;

        }

        public async Task<List<GetFInanceCircle>?> AllFinanceCircle(string username)
        {
            var userId = await _userIdService.GetUserId(username);
            if (userId == null)
            {
                // Log the error

                return null;
            }

            var bankAccountId = await _userIdService.GetBankAccountNumber(userId);
            if (bankAccountId == null)
            {
                // Log the error

                return null;
            }

            var financeCircles = await _context.FinanceCircleData
                .Where(circle => circle.Friends.Contains(bankAccountId) || circle.CreatorId == bankAccountId)
                .ToListAsync();
            var consoleOut = JsonConvert.SerializeObject(financeCircles, Formatting.Indented);

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
                    Friends = friendInfos.Take(6).ToList(),


                    CreatorId = circle.CreatorId,


                };

                circleList.Add(circleInfo);
            }

            var circleDtoList = circleList.Select(circle => circle.ToGetAllFinanceCircleDto()).ToList();

            return circleDtoList;

        }

        public async Task<CircleResponseDto?> CreateFinanceCircle(CreateFinanceCircleDto circleInfo, string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                var userBankAccountId = await _userIdService.GetBankAccountNumber(userId);
                if (userBankAccountId is null)
                {
                    return null;
                }
                circleInfo.CircleId = Guid.NewGuid();
                circleInfo.CreatorId = userBankAccountId;
                circleInfo.TotalCommittment += circleInfo.PersonalCommittmentPercentage;
                circleInfo.Friends.Add(userBankAccountId);
                bool isEligible = await _serviceComputations.UpdateUsersCommittments(circleInfo.CreatorId, circleInfo.PersonalCommittmentPercentage);

                if (isEligible)
                {

                    var result = await AddCommitment(userBankAccountId, circleInfo.CircleId, circleInfo.PersonalCommittmentPercentage);

                    await _context.FinanceCircleData.AddAsync(circleInfo.ToCreateFinanceCircleDto());
                    await _context.SaveChangesAsync();
                    return new CircleResponseDto
                    {
                        Message = "Group Created",
                        CircleId = circleInfo.CircleId
                    };
                }
                else
                {
                    return new CircleResponseDto
                    {
                        Message = "You do not have enough fund to create group. Update your amount"
                    };
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
            double totalCommittmentValue = (double)((double)balanceInfo.TotalCommittment / 100 * balanceInfo.CurrentBalance);

            if (totalCommittmentValue > 5)
            {
                return new EligibilityResult { Result = true };
            }
            else
            {
                return new EligibilityResult { Result = false, Message = "Your current balance may not support this new circle. Please top up" };
            }

        }

        public async Task<GetFInanceCircle?> SingleFinanceCircle(Guid circleId)
        {
            var financeCircle = await _context.FinanceCircleData
                .FirstOrDefaultAsync(circle => circle.CircleId == circleId);

            if (financeCircle == null)
            {
                throw new InvalidCastException("Finance circle not found.");
            }

            var friendInfos = await GetFriendInformation(financeCircle.Friends);

            var circle = new GetFInanceCircle
            {
                CircleId = financeCircle.CircleId,
                Name = financeCircle.Name,
                Friends = friendInfos.Take(6).ToList(),
                FundWithdrawalApprovalCount = financeCircle.FundWithdrawalApprovalCount,
                WithdrawalChargePercentage = financeCircle.WithdrawalChargePercentage,
                PersonalCommittmentPercentage = financeCircle.PersonalCommittmentPercentage,
                CreatorId = financeCircle.CreatorId,
                CircleType = (DTOS.FinanceCircle.CircleType)financeCircle.CircleType,
                TargetAmount = financeCircle.TargetAmount,
                TotalCommittment = financeCircle.TotalCommittment

            };
            var circleDto = circle.ToGetFinanceCircleDto();
            return circleDto;
        }

        private async Task<List<FriendInformation>> GetFriendInformation(IEnumerable<string> bankIds)
        {
            var friendInfos = new List<FriendInformation>();

            foreach (var bankId in bankIds)
            {
                try
                {
                    var accountInfo = await _userIdService.GetBankAccountInfo(bankId);
                    if (accountInfo?.KallumUser != null)
                    {
                        friendInfos.Add(new FriendInformation
                        {
                            UserName = accountInfo.KallumUser.UserName,
                            Email = accountInfo.KallumUser.Email
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return friendInfos;
        }
        private async Task<CircleResponseDto?> AddCommitment(string bankId, Guid circleId, double percentage)
        {
            try
            {
                // Retrieve the finance circle information based on the circleId

                DateTime dateTime = DateTime.UtcNow;

                // Create the response object with activity and commitment history
                var response = new CreateCircleActivity
                {
                    Activity = new DTOS.CircleDto.Activity
                    {
                        ActivityId = Guid.NewGuid(),
                        BankId = bankId,
                        DateTime = dateTime,
                        ActivityType = 0
                    },
                    CircleId = circleId,
                    CommitmentHistory = new DTOS.CircleDto.CommitmentHistory
                    {
                        DateTime = dateTime,
                        TransactionId = Guid.NewGuid(),
                        Percentage = percentage
                    }
                };

                // Update the total commitment


                // Add the response to the CircleData
                await _context.CircleData.AddAsync(response.ToCreateCircleActivityDto());
                await _context.SaveChangesAsync();

                // Return the response DTO
                return new CircleResponseDto
                {
                    Message = "Done"
                    // Add other properties if necessary
                };

            }
            catch (Exception e)
            {
                // Log the exception if necessary and throw a new exception with a meaningful message
                throw new Exception($"An error occurred while adding the commitment: {e.Message}", e);
            }
        }

    }
}