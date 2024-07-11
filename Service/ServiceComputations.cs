using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
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

        public ServiceComputations(UserManager<AppUser> userManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
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
        public async Task<bool> UpdateUsersCommittments(string bankAccountId, double committment)
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

            if (IsBalanceEnoughForCommitment(balanceInfo.CurrentBalance, balanceInfo.TotalCommittment, committment))
            {
                // Update the total commitment
                balanceInfo.TotalCommittment += committment;

                // Update existing entity in the context
                _context.Update(balanceInfo);
                await _context.SaveChangesAsync();

                return true; // Commitment updated successfully
            }
            else
            {
                return false; // Insufficient balance for the commitment
            }
        }


    }
}