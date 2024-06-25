using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS;
using Kallum.DTOS.Bank;
using Kallum.Models;

namespace Kallum.Interfaces
{
    public interface IBankOperationRepository
    {
        Task<BankAccountDto?> CreateBankAccount(string userId);
        Task<BalanceDetails?> GetBalanceDetails(string username);
        Task<BankAccountDto?> GetBankAccountAsync(string bankId);
    }
}