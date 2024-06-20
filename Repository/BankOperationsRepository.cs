using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS;
using Kallum.DTOS.Bank;
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
            for (int i = 0; i < 10; i++)
            {
                accountNumber += random.Next(0, 10).ToString();

            }
            return accountNumber;
        }

        public async Task<BankAccountDto> CreateBankAccount(string userId)
        {
            try
            {
                var generatedAccountNumber = GenerateaBankAcccountNumber();
                var bankDetails = new BankAccount
                {
                    AccountType = "Savings",
                    BankAccountId = generatedAccountNumber,
                    CreatedDate = DateTime.Now,
                    UserAccountId = userId,
                    Status = "Active"


                };
                var userBankAccountAlreadyExists = await _context.BankAccountsData.AnyAsync(user => user.UserAccountId == userId);
                if (userBankAccountAlreadyExists)
                {
                    var userBankAccountInfo = await _context.BankAccountsData.FirstOrDefaultAsync(user => user.UserAccountId == userId);

                    return userBankAccountInfo.ToBankAccountDto();

                }
                await _context.BankAccountsData.AddAsync(bankDetails);
                await _context.SaveChangesAsync();
                var bankDetailsDto = new BankAccountDto
                {
                    AccountType = "Savings",
                    BankAccountId = generatedAccountNumber,
                    CreatedDate = DateTime.Now,
                    Status = "Active"

                };
                return bankDetailsDto;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<BalanceDetails> GetBalanceDetails(string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                var bankDetailsData = await _context.BalanceDetailsData.Include(account => account.BankAccountDetails).FirstOrDefaultAsync(details => details.BankAccountDetails.UserAccountId == userId);
                return bankDetailsData;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }





        }

        public async Task<KallumLockDto> GetKallumLockStatus(string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                var KallumLock = await _context.KallumLockData.FirstOrDefaultAsync(details => details.UserAccountId == userId);
                return KallumLock.ToKallumLockDto();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<string> SetKallumLock(string username, KallumLockDto lockdetails)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                var kallumLock = new KallumLock
                {
                    SecurePin = lockdetails.SecurePin,
                    TransactionPin = lockdetails.TransactionPin,
                    UserAccountId = userId
                };

                await _context.KallumLockData.AddAsync(kallumLock);
                await _context.SaveChangesAsync();
                return "Done";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}