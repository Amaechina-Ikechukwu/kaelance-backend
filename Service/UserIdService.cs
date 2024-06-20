using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS;
using Kallum.Mappers;
using Kallum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Service
{
    public class UserIdService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDBContext _context;

        public UserIdService(UserManager<AppUser> userManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> GetUserId(string username)
        {
            var userInfo = await _userManager.FindByNameAsync(username);
            if (userInfo == null)
            {
                throw new KeyNotFoundException($"User with username '{username}' not found.");
            }

            return userInfo.Id;
        }

        public async Task<string> GetBankAccountNumber(string userId)
        {
            var accountInfo = await _context.BankAccountsData.FirstOrDefaultAsync(user => user.UserAccountId == userId);
            return accountInfo.BankAccountId;
        }
        public async Task<BankAccountDto> GetBankAccountInfo(string bankAccountId)
        {
            var accountInfo = await _context.BankAccountsData.FirstOrDefaultAsync(user => user.BankAccountId == bankAccountId);
            return accountInfo.ToBankAccountDto();
        }
    }
}
