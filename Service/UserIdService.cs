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
        public async Task<AppUser> GetUserFullInformation(string username)
        {
            var userInfo = await _userManager.FindByNameAsync(username);
            if (userInfo == null)
            {
                throw new KeyNotFoundException($"User with username '{username}' not found.");
            }

            return userInfo;
        }
        public async Task<string?> GetUserName(string userId)
        {
            var userInfo = await _userManager.FindByIdAsync(userId);
            if (userInfo == null)
            {
                throw new KeyNotFoundException($"User with username '{userId}' not found.");
            }

            return userInfo.UserName;
        }
        public async Task<string?> GetUserEmail(string userId)
        {
            var userInfo = await _userManager.FindByIdAsync(userId);
            if (userInfo == null)
            {
                throw new KeyNotFoundException($"User with username '{userId}' not found.");
            }

            return userInfo.Email;
        }
        public async Task<string?> GetIdByUserEmail(string email)
        {
            var userInfo = await _userManager.FindByEmailAsync(email);
            if (userInfo == null)
            {

                return null;
            }

            return userInfo.Id;
        }


        public async Task<string?> GetBankAccountNumber(string userId)
        {
            var accountInfo = await _context.BankAccountsData.FirstOrDefaultAsync(user => user.AppUser.Id == userId);
            if (accountInfo is null)
            {
                throw new KeyNotFoundException($"User with username '{accountInfo}' not found.");
            }
            return accountInfo.BankAccountId;
        }
        public async Task<string?> GetBankAccountNumberWithUsername(string username)
        {
            var accountInfo = await _context.BankAccountsData.FirstOrDefaultAsync(user => user.AppUser.UserName == username);
            if (accountInfo is null)
            {
                throw new KeyNotFoundException($"User with username '{accountInfo}' not found.");
            }
            return accountInfo.BankAccountId;
        }
        public async Task<BankAccountDto?> GetBankAccountInfo(string bankAccountId)
        {
            var queryUsers = _context.BankAccountsData.AsQueryable();

            if (!string.IsNullOrWhiteSpace(bankAccountId))
            {
                queryUsers = queryUsers.Where(user => user.BankAccountId.Contains(bankAccountId));
            }



            var result = await queryUsers
                .Select(user => new BankAccountDto
                {
                    AppUserId = user.AppUserId,

                    BankAccountId = user.BankAccountId,
                    KallumUser = new AppUserDto
                    {
                        Email = user.AppUser.Email,
                        UserName = user.AppUser.UserName,

                        // Add other fields as necessary
                    }
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<BalanceDetails?> GetBalanceDetailsWithBankId(string bankId)
        {
            try
            {
                var bankDetailsData = await _context.BalanceDetailsData
                .Where(ba => ba.BankAccountDetails.BankAccountId == bankId)
                .Select(ba => new BalanceDetails
                {
                    Currency = ba.Currency,
                    CurrencySymbol = ba.CurrencySymbol,
                    CurrentBalance = ba.CurrentBalance ?? 0.0,
                    Id = ba.Id,
                    TotalCommittment = ba.TotalCommittment,
                    LastUpdated = ba.LastUpdated,
                    BankAccountDetails = ba.BankAccountDetails


                })
                .FirstOrDefaultAsync();

                if (bankDetailsData is null)
                {
                    return null;
                }
                return bankDetailsData;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }





        }

    }
}
