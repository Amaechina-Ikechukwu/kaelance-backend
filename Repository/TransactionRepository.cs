using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.Notifications;
using Kallum.DTOS.Transactions;
using Kallum.Helper;
using Kallum.Models;
using Kallum.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Repository
{
    public class TransactionRepository : ITransactionsRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public readonly ServiceComputations _serviceComputations;
        public TransactionRepository(ApplicationDBContext context, UserIdService userIdService, ServiceComputations serviceComputations)
        {
            _context = context;
            _userIdService = userIdService;
            _serviceComputations = serviceComputations;
        }

        public async Task<TransactionHistory?> BloatAccount(string reciever, double amount)
        {
            try
            {
                var updateAccountResult = await UpdateReceiversAccount(reciever, amount);
                var transaction = new TransactionHistory
                {
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    RecieverId = reciever,
                    SenderId = "Oversight",
                    TransactionDescription = "Initialization of the Kallum Economy",
                    TransactionHistoryId = Guid.NewGuid(),
                    Currency = "Naira",
                    CurrencySymbol = "#"

                };
                if (updateAccountResult == null)
                {
                    return null;
                }

                await _context.TransactionHistoriesData.AddAsync(transaction);

                await _context.SaveChangesAsync();
                return transaction;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<BalanceDetails> UpdateReceiversAccount(string receiver, double amount)
        {
            var accountBalanceInfo = await _context.BankAccountsData.FirstOrDefaultAsync(account => account.BankAccountId == receiver);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var balanceInfo = await _context.BalanceDetailsData.FirstOrDefaultAsync(balance => balance.BankAccountDetails.BankAccountId == receiver);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (balanceInfo is null)
            {
                var newTransaction = new BalanceDetails
                {
                    BankAccountDetails = accountBalanceInfo,
                    CurrentBalance = amount,
                    LastUpdated = DateTime.UtcNow,
                    Currency = "Naira",
                    CurrencySymbol = "#"
                };
                await _context.BalanceDetailsData.AddAsync(newTransaction);
                await _context.SaveChangesAsync();

                return newTransaction;
            }
            else
            {
                balanceInfo.CurrentBalance += amount;
                balanceInfo.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return balanceInfo;
            }
        }


        public async Task<TransactionHistory> SendMoney(CreateTransactionDto transactionDto, string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var senderInfo = await _context.BankAccountsData.FirstOrDefaultAsync(account => account.AppUser.Id == userId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                transactionDto.SenderId = senderInfo.BankAccountId;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8604 // Possible null reference argument.
                await DeductSendersBalance(transactionDto.SenderId, transactionDto.Amount);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
                await UpdateReceiversAccount(transactionDto.RecieverId, transactionDto.Amount);
#pragma warning restore CS8604 // Possible null reference argument.
                var transactionResult = await AddToTransactionsHistory(transactionDto);
                return transactionResult;
            }
            catch (Exception e)
            {

                throw new Exception(e.ToString());
            }
        }
        public async Task<BalanceDetails?> DeductSendersBalance(string senderId, double amount)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var balanceInfo = await _context.BalanceDetailsData.FirstOrDefaultAsync(balance => balance.BankAccountDetails.BankAccountId == senderId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (balanceInfo is null)
            {
                return null;
            }
            balanceInfo.CurrentBalance = balanceInfo.CurrentBalance - amount;
            balanceInfo.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return balanceInfo;
        }
        public async Task<TransactionHistory> AddToTransactionsHistory(CreateTransactionDto transactionDto)
        {
            var transaction = new TransactionHistory
            {
                Amount = transactionDto.Amount,
                Date = DateTime.UtcNow,
                RecieverId = transactionDto.RecieverId,
                SenderId = transactionDto.SenderId,
                TransactionDescription = transactionDto.TransactionDescription,
                TransactionHistoryId = Guid.NewGuid(),
                Currency = "Naira",
                CurrencySymbol = "#"

            };


            await _context.TransactionHistoriesData.AddAsync(transaction);

            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<TransactionHistoryDto>?> GetTransactionHistory(string username)
        {
            var userId = await _userIdService.GetUserId(username);
            var userBankAccountId = await _userIdService.GetBankAccountNumber(userId);
            var transactionInformation = await _context.TransactionHistoriesData
                .Where(transactions => transactions.RecieverId == userBankAccountId || transactions.SenderId == userBankAccountId)
                .ToListAsync();
            if (transactionInformation is null)
            {
                return null;
            }

            var completeTransactionInformation = new List<TransactionHistoryDto>();

            foreach (var transaction in transactionInformation)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var receiverInfoTask = await _userIdService.GetBankAccountInfo(transaction.RecieverId) ?? null;
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
                var senderInfoTask = await _userIdService.GetBankAccountInfo(transaction.SenderId) ?? null;
#pragma warning restore CS8604 // Possible null reference argument.



                var receiverInfo = receiverInfoTask;
                var senderInfo = senderInfoTask;

                var transactionDto = new TransactionHistoryDto
                {
                    Amount = transaction.Amount,
                    Currency = transaction.Currency,
                    CurrencySymbol = transaction.CurrencySymbol,
                    Date = transaction.Date,
                    Reciever = receiverInfo,
                    Sender = senderInfo,
                    TransactionDescription = transaction.TransactionDescription,
                    TransactionHistoryId = transaction.TransactionHistoryId,
                    TransationType = transaction.RecieverId == userBankAccountId ? "Credit" : "Debit"
                };
                completeTransactionInformation.Add(transactionDto);
            }
            return completeTransactionInformation;
        }
        public async Task<string> TopUpWeebhook(ChargeCompletedEvent webhookEvent)
        {
            try
            {
                // Retrieve the customer ID using the email from the webhook event
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string customerId = await _userIdService.GetIdByUserEmail(webhookEvent.Customer.Email);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.

                // Check if the customer ID is null
                if (customerId is null)
                {
                    return "Customer not found";
                }
                string bankId = await _userIdService.GetBankAccountNumber(customerId);
                if (bankId is null)
                {
                    return "Bank Id not found";
                }
                // Retrieve the amount from the webhook event
                double amount = webhookEvent.Amount;
                NotificationDto notification = new NotificationDto
                {
                    DateTime = DateTime.UtcNow,
                    SeenNotification = false,
                    Title = $"{amount} added to your kaelance balance",
                    Type = "Transactions",
                    TypeId = $"{webhookEvent.TxRef}",
                    BankId = bankId

                };
                // Update the receiver's account with the specified amount
                await UpdateReceiversAccount(bankId, amount);
                await _serviceComputations.AddNotification(bankId, notification);

                return "Account updated";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

        }

    }
}
