using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.Transactions;
using Kallum.Helper;
using Kallum.Models;

namespace Kallum.Repository
{
    public interface ITransactionsRepository
    {
        Task<TransactionHistory> SendMoney(CreateTransactionDto transactionDto, string username);
        Task<List<TransactionHistoryDto>> GetTransactionHistory(string username);

        Task<TransactionHistory> BloatAccount(string reciever, decimal amount);
        Task<string> TopUpWeebhook(ChargeCompletedEvent webhookEvent);
    }
}