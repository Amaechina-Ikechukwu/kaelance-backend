using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS;
using Kallum.DTOS.Bank;
using Kallum.Helper;
using Kallum.Interfaces;
using Kallum.Mappers;
using Kallum.Models;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Service
{


    public class BankOperationRepository : IBankOperationRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public BankOperationRepository(ApplicationDBContext context, UserIdService userIdService)
        {
            _context = context;
            _userIdService = userIdService;
        }
        public static string GenerateaBankAcccountNumber()
        {

            Random random = new Random();
            string accountNumber = string.Empty;
            for (double i = 0; i < 10; i++)
            {
                accountNumber += random.Next(0, 10).ToString();

            }
            return accountNumber;
        }

        public async Task<BankAccountDto?> CreateBankAccount(string userId)
        {
            try
            {
                var generatedAccountNumber = GenerateaBankAcccountNumber();
                var bankDetails = new BankAccount
                {
                    AccountType = "Savings",
                    BankAccountId = generatedAccountNumber,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Active",
                    AppUserId = userId,
                    Id = "kallum-" + Guid.NewGuid()
                };

                var userBankAccountAlreadyExists = await _context.BankAccountsData.AnyAsync(user => user.AppUserId == userId);
                if (userBankAccountAlreadyExists)
                {
                    var userBankAccountInfo = await _context.BankAccountsData
                        .Where(user => user.AppUserId == userId)
                        .Select(user => user.ToBankAccountDto())
                        .FirstOrDefaultAsync();

                    return userBankAccountInfo;
                }

                await _context.BankAccountsData.AddAsync(bankDetails);
                await _context.SaveChangesAsync();

                var bankDetailsDto = new BankAccountDto
                {
                    BankAccountId = generatedAccountNumber,
                    Status = "Active",
                    KallumUser = null // Assuming there is no AppUser information for new accounts
                };

                return bankDetailsDto;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public async Task<List<BankAccountDto?>> FindBankUser(FinanceCircleQueryObject query)
        {
            var queryUsers = _context.BankAccountsData.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.BankAccountId))
            {
                queryUsers = queryUsers.Where(user => user.BankAccountId.Contains(query.BankAccountId));
            }

            if (!string.IsNullOrWhiteSpace(query.UserName))
            {
                queryUsers = queryUsers.Where(user => user.AppUser.UserName.ToLower().Contains(query.UserName.ToLower()));
            }

            var result = await queryUsers
                .Select(user => new BankAccountDto
                {

                    BankAccountId = user.BankAccountId,
                    KallumUser = new AppUserDto
                    {
                        Email = user.AppUser.Email,
                        UserName = user.AppUser.UserName,

                        // Add other fields as necessary
                    }
                })
                .ToListAsync();

            return result;
        }


        public async Task<BalanceDetails?> GetBalanceDetails(string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                var bankDetailsData = await _context.BalanceDetailsData
                .Where(ba => ba.BankAccountDetails.AppUserId == userId)
                .Select(ba => new BalanceDetails
                {
                    Currency = ba.Currency,
                    CurrencySymbol = ba.CurrencySymbol,
                    CurrentBalance = ba.CurrentBalance ?? 0.0,
                    Id = ba.Id,
                    TotalCommittment = ba.TotalCommittment,
                    LastUpdated = ba.LastUpdated


                })
                .FirstOrDefaultAsync();

                if (bankDetailsData is null)
                {
                    return new BalanceDetails
                    {
                        Currency = "#",
                        CurrencySymbol = "#",
                        CurrentBalance = 0.0,
                        Id = 0

                    };
                }
                return bankDetailsData;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }





        }

        public async Task<BankAccountDto?> GetBankAccountAsync(string bankid)
        {
            return await _context.BankAccountsData
                .Where(ba => ba.BankAccountId == bankid)
                .Select(ba => new BankAccountDto
                {
                    BankAccountId = ba.BankAccountId,
                    Status = ba.Status,
                    AccountType = ba.AccountType,
                    KallumUser = new AppUserDto
                    {

                        Email = ba.AppUser.Email,
                        UserName = ba.AppUser.UserName
                    }
                })
                .FirstOrDefaultAsync();
        }


    }
}