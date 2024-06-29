using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public bool IsBalanceEnoughForCommittment(decimal? currentBalance, int totalcommittment, int committment)
        {
            if (currentBalance is null)
            {
                return false;
            }

            decimal totalCommittmentValue = (decimal)((decimal)totalcommittment / 100 * currentBalance);
            decimal newCommittmentValue = (decimal)committment / 100 * totalCommittmentValue;
            if (newCommittmentValue > 1)
            {
                return false;
            }
            else
            {
                return true;
            }


        }
        public async Task<bool> UpdateUsersCommittments(string bankAccountId, int committment)
        {
            var balanceInfo = await _context.BalanceDetailsData
                .Include(b => b.BankAccountDetails)
                .FirstOrDefaultAsync(b => b.BankAccountDetails.BankAccountId == bankAccountId);

            if (balanceInfo == null)
            {
                return false; // Balance information not found
            }

            if (IsBalanceEnoughForCommittment(balanceInfo.CurrentBalance, balanceInfo.TotalCommittment, committment))
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