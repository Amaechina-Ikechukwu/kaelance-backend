using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS;
using Kallum.DTOS.CircleDto;
using Kallum.DTOS.Notifications;
using Kallum.Helper;
using Kallum.Interfaces;
using Kallum.Mappers;
using Kallum.Models;
using Kallum.Service;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Repository
{
    public class CircleRepository : ICircleRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public readonly ServiceComputations _serviceComputations;

        public CircleRepository(ApplicationDBContext context, UserIdService userIdService, ServiceComputations serviceComputation)
        {
            _context = context;
            _userIdService = userIdService;
            _serviceComputations = serviceComputation;
        }

        public async Task<CircleResponseDto?> AddCommitment(string bankId, Guid circleId, double percentage)
        {
            try
            {
                // Retrieve the finance circle information based on the circleId
                var financeCircleInfo = await _context.FinanceCircleData
                    .Where(info => info.CircleId == circleId)
                    .FirstOrDefaultAsync();

                if (financeCircleInfo is null)
                {
                    return null; // Return null if the circle does not exist
                }

                // Check if the bankId is part of the circle's friends
                if (financeCircleInfo.Friends.Contains(bankId))
                {
                    if (financeCircleInfo.TotalCommittment >= 100 ||
     financeCircleInfo.TotalCommittment + percentage > 100)
                    {
                        throw new InvalidOperationException("Contribution cannot exceed target percentage or amount");
                    }
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
                    if (bankId == financeCircleInfo.CreatorId)
                    {
                        financeCircleInfo.PersonalCommittmentPercentage += percentage;
                    }
                    // Update the total commitment
                    financeCircleInfo.TotalCommittment += percentage;
                    NotificationDto notification = new NotificationDto
                    {
                        DateTime = dateTime,
                        SeenNotification = false,
                        Title = $"{percentage}% added to {financeCircleInfo.Name}",
                        Type = "Circle",
                        TypeId = $"{financeCircleInfo.CircleId}",
                        BankId = bankId

                    };

                    _context.FinanceCircleData.Update(financeCircleInfo);

                    // Add the response to the CircleData
                    await _context.CircleData.AddAsync(response.ToCreateCircleActivityDto());
                    await _context.SaveChangesAsync();
                    await _serviceComputations.AddNotification(bankId, notification);
                    // Return the response DTO
                    return new CircleResponseDto
                    {
                        Message = "Done"
                        // Add other properties if necessary
                    };
                }
                else
                {
                    // Handle the case where bankId is not part of the circle's friends
                    throw new InvalidOperationException("BankId is not part of the circle's friends.");
                }
            }
            catch (Exception e)
            {
                // Log the exception if necessary and throw a new exception with a meaningful message
                throw new Exception($"An error occurred while adding the commitment: {e.Message}", e);
            }
        }

        public async Task<CircleResponseDto?> InitiatePersonalWithdrawalFromCircle(string username, Guid circleId)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                string bankId = await _userIdService.GetBankAccountNumber(userId);
                if (bankId is null)
                {
                    return null;
                }
                DateTime dateTime = DateTime.UtcNow;
                var response = new CreateCircleActivity
                {
                    Activity = new DTOS.CircleDto.Activity
                    {
                        ActivityId = Guid.NewGuid(),
                        BankId = bankId,
                        DateTime = dateTime,
                        ActivityType = 0,
                    },
                    CircleId = circleId,
                    WithdrawalAction = new DTOS.CircleDto.WithdrawalAction
                    {
                        DateTime = dateTime,
                        WithdrawalType = 0
                    },
                };

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Circle circleInfo = await _context.CircleData.Where(info => info.Activity.BankId == bankId).FirstOrDefaultAsync();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (circleInfo is null)
                {
                    return null;
                }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                BalanceDetails balanceInfo = await _context.BalanceDetailsData.Where(info => info.BankAccountDetails.BankAccountId == bankId).FirstOrDefaultAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (balanceInfo is null)
                {
                    return null;
                }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var newBalanceInfo = new BalanceDetails
                {
                    BankAccountDetails = balanceInfo.BankAccountDetails,
                    Currency = balanceInfo.Currency,
                    CurrencySymbol = balanceInfo.CurrencySymbol,
                    CurrentBalance = balanceInfo.CurrentBalance,
                    LastUpdated = DateTime.UtcNow,
                    TotalCommittment = balanceInfo.TotalCommittment,
                    DeclinmentCount = balanceInfo.DeclinmentCount + 2
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                await UpdateFriendsList(circleId, bankId, "remove");
                await _context.CircleData.AddAsync(response.ToCreateCircleActivityDto());
                await _context.SaveChangesAsync();
                return new CircleResponseDto
                {
                    // Populate the properties of your response DTO as needed
                    Message = "Done"
                    // Add other properties if necessary
                }; ;
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }
        }

        public async Task<CircleResponseDto?> InitiateCircleWithdrawal(string username, Guid circleId)
        {

            try
            {
                var userId = await _userIdService.GetUserId(username);
                string bankId = await _userIdService.GetBankAccountNumber(userId);
                var creatorId = await _context.FinanceCircleData
                                                 .Where(circle => circle.CircleId == circleId).Select(s => s.CreatorId)
                                                 .FirstOrDefaultAsync();
                if (bankId is null || creatorId is null)
                {
                    return null;
                }
                DateTime dateTime = DateTime.UtcNow;

                var response = new CreateCircleActivity
                {
                    Activity = new DTOS.CircleDto.Activity
                    {
                        ActivityId = Guid.NewGuid(),
                        BankId = bankId,
                        DateTime = dateTime,
                        ActivityType = (DTOS.CircleDto.ActivityType)1,


                    },
                    CircleId = circleId,
                    WithdrawalAction = new DTOS.CircleDto.WithdrawalAction
                    {
                        DateTime = dateTime,
                        WithdrawalType = (DTOS.CircleDto.WithdrawalType)1,
                        ApprovalByAll = 1,
                        ApprovedByCreator = true
                    },


                };
                await _context.CircleData.AddAsync(response.ToCreateCircleActivityDto());
                await _context.SaveChangesAsync();
                return new CircleResponseDto
                {
                    // Populate the properties of your response DTO as needed
                    Message = "You have requested for withdrawal. Alert others to approve"
                    // Add other properties if necessary
                }; ;
            }
            catch (Exception e)
            {

                throw new Exception(e.ToString());
            }

        }

        public async Task<List<CreateCircleActivity>> ActivityHistory(Guid circleId)
        {
            // Retrieve circle information based on the given circleId and map directly to DTO
            var circleInfo = await _context.CircleData.Include(info => info.Activity).Include(info => info.CommitmentHistory).Include(info => info.WithdrawalAction)
                                           .Where(circle => circle.CircleId == circleId)
                                           .Select(circle => circle.ToReturnCircleActivity())
                                           .ToListAsync();

            // Return an empty list if circleInfo is null or empty
            return circleInfo ?? new List<CreateCircleActivity>();
        }


        public async Task<CircleResponseDto?> UpdateFriendsList(Guid circleId, string bankId, string type)
        {
            try
            {
                // Retrieve the specific FinanceCircle entity
                var financeCircle = await _context.FinanceCircleData
                                                  .Where(circle => circle.CircleId == circleId)
                                                  .FirstOrDefaultAsync();


                var bankInfo = await _userIdService.GetBankAccountInfo(bankId);
                var balanceInfo = await _userIdService.GetBalanceDetailsWithBankId(bankId);

                if (financeCircle == null)
                {
                    throw new Exception("Finance circle not found.");
                }
                if (balanceInfo == null)
                {
                    throw new Exception("BalanceInfo not found.");
                }
                if (financeCircle.CreatorId == bankId)
                {
                    throw new Exception("Cannot remove the creator.");
                }

                // Modify the friends list based on the type
                if (type == "remove")
                {
                    var notification = new NotificationDto
                    {
                        DateTime = DateTime.UtcNow,
                        SeenNotification = false,
                        Title = $"{bankInfo.KallumUser.UserName} removed from {financeCircle.Name} finance circle",
                        Type = "Circle",
                        TypeId = $"{financeCircle.CircleId}",
                        BankId = bankId
                    };

                    var totalCommitmentByUser = await UserTotalCommitmentToACircle(circleId, bankId) ?? 0.0;
                    financeCircle.TotalCommittment = financeCircle.TotalCommittment - totalCommitmentByUser;
                    balanceInfo.TotalCommittment = balanceInfo.TotalCommittment - totalCommitmentByUser;

                    financeCircle.Friends.Remove(bankId);
                    foreach (var friend in financeCircle.Friends)
                    {
                        await _serviceComputations.AddNotification(friend, notification);
                    }

                }
                else if (type == "add")
                {
                    var notification = new NotificationDto
                    {
                        DateTime = DateTime.UtcNow,
                        SeenNotification = false,
                        Title = $"{bankInfo.KallumUser.UserName} added to {financeCircle.Name} finance circle",
                        Type = "Circle",
                        TypeId = $"{financeCircle.CircleId}",
                        BankId = bankId
                    };

                    financeCircle.Friends.Add(bankId);
                    foreach (var friend in financeCircle.Friends)
                    {
                        await _serviceComputations.AddNotification(friend, notification);
                    }
                }
                else
                {
                    throw new Exception("Invalid type specified. Use 'add' or 'remove'.");
                }

                // Save changes to the entity
                _context.FinanceCircleData.Update(financeCircle);
                _context.BalanceDetailsData.Update(balanceInfo);
                await _context.SaveChangesAsync();

                // Create and return the response DTO (assuming you have a way to create this DTO)
                var responseDto = new CircleResponseDto
                {
                    Message = "Updated"
                };

                return responseDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }
        public async Task<CircleResponseDto?> CommitToCircle(Guid circleId, string username, double percentage)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                string bankId = await _userIdService.GetBankAccountNumber(userId);
                if (bankId is null)
                {
                    return null;
                }
                var financeCircle = await _context.FinanceCircleData
               .FirstOrDefaultAsync(circle => circle.CircleId == circleId);

                if (financeCircle == null)
                {
                    throw new InvalidCastException("Finance circle not found.");
                }
                bool isEligible = await _serviceComputations.UpdateUsersCommittments(bankId, percentage, financeCircle.TargetAmount);

                if (isEligible)
                {
                    DateTime dateTime = DateTime.UtcNow;
                    CircleResponseDto response = await AddCommitment(bankId, circleId, percentage);
                    return response;
                }
                else
                {
                    throw new InvalidCastException("You do not have Enough balance");
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<CircleResponseDto?> InitiateWidthdrawl(Guid circleId, string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                string bankId = await _userIdService.GetBankAccountNumber(userId);
                if (bankId == null)
                {
                    return null;
                }
                var creatorId = await _context.FinanceCircleData
                                                                  .Where(circle => circle.CircleId == circleId).Select(s => s.CreatorId)
                                                                  .FirstOrDefaultAsync();
                var circleInfo = await _context.CircleData
                                               .Where(c => c.CircleId == circleId).FirstOrDefaultAsync();
                if (circleInfo is null && creatorId is null && bankId is null)
                {
                    return null;
                }


                if (creatorId == bankId)
                {
                    circleInfo.WithdrawalApprovalPercentage += 1;
                    return new CircleResponseDto
                    {
                        Message = "You have requested for withdrawal. Alert others to approve"
                    };
                }
                else
                {
                    throw new InvalidCastException("You cannot initiate withdrawal");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        private async Task<double?> UserTotalCommitmentToACircle(Guid circleId, string bankId)
        {
            // Fetch the circle history data from the database
            List<Circle> history = await _context.CircleData
                .Where(circle => circle.CircleId == circleId && circle.Activity.BankId == bankId)
                .ToListAsync();

            // If no history is found, return null
            if (history == null || !history.Any())
            {
                return null;
            }

            // Initialize total commitment
            double total = 0;

            // Sum up the commitment percentages from the history
            foreach (var circle in history)
            {
                if (circle.CommitmentHistory != null)
                {
                    total += circle.CommitmentHistory.Percentage;
                }
            }

            // Return the total commitment
            return total;
        }

    }

}