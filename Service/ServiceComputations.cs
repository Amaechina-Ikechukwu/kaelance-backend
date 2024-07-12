using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.Notifications;
using Kallum.Helper;
using Kallum.Mappers;
using Kallum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Kallum.Service
{
    public class ServiceComputations
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDBContext _context;
        private readonly UserIdService _userIdService;

        public ServiceComputations(UserManager<AppUser> userManager, ApplicationDBContext context, UserIdService userIdService)
        {
            _userManager = userManager;
            _context = context;
            _userIdService = userIdService;
        }
        public bool IsBalanceEnoughForCommitment(double? currentBalance, double totalCommitmentPercentage, double newCommitmentPercentage)
        {
            if (currentBalance == null || currentBalance <= 0)
            {
                return false;
            }

            double totalCommitmentValue = currentBalance.Value * (totalCommitmentPercentage / 100);
            double newCommitmentValue = (currentBalance.Value - totalCommitmentValue) * (newCommitmentPercentage / 100);

            if (newCommitmentValue < 0)
            {
                return false;
            }
            return newCommitmentValue <= currentBalance;
        }
        public async Task<bool> UpdateUsersCommittments(string bankAccountId, double committment, double targetAmount)
        {

            var balanceInfo = await _context.BalanceDetailsData
                .Where(b => b.BankAccountDetails.BankAccountId == bankAccountId).Select(ba => new BalanceDetails
                {
                    Currency = ba.Currency,
                    CurrencySymbol = ba.CurrencySymbol,
                    CurrentBalance = ba.CurrentBalance ?? 0.0,
                    Id = ba.Id,
                    TotalCommittment = ba.TotalCommittment,
                    LastUpdated = ba.LastUpdated


                })
                .FirstOrDefaultAsync();
            if (balanceInfo == null)
            {
                return false; // Balance information not found
            }

            // Ensure CurrentBalance is not null before division
            if (balanceInfo.CurrentBalance.HasValue && balanceInfo.CurrentBalance.Value != 0)
            {
                var percentageAmount = committment / 100 * targetAmount;
                double calculatedPercentage = percentageAmount / balanceInfo.CurrentBalance.Value * 100;

                // Update the total commitment
                balanceInfo.TotalCommittment += calculatedPercentage;

                // Update existing entity in the context
                _context.Update(balanceInfo);
                await _context.SaveChangesAsync();

                return true; // Commitment updated successfully
            }
            else
            {
                // Handle the case where CurrentBalance is null or zero
                // You might want to log an error or throw an exception here
                return false; // Or handle the error as needed
            }


        }
        public async Task<bool> AddNotification(string bankId, NotificationDto notification)

        {

            try
            {

                if (bankId is null)
                {
                    return false;
                }
                await _context.Notifications.AddAsync(notification.ToCreateNotificationDto(bankId));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


    }
}