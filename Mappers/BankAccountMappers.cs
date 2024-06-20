using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS;
using Kallum.DTOS.Bank;
using Kallum.Models;

namespace Kallum.Mappers
{
    public static class BankAccountMappers
    {
        public static BankAccountDto ToBankAccountDto(this BankAccount bankAccount)
        {
            return new BankAccountDto
            {
                AccountType = bankAccount.AccountType,
                CreatedDate = bankAccount.CreatedDate,
                Status = bankAccount.Status,
                BankAccountId = bankAccount.BankAccountId

            };
        }
        public static KallumLockDto ToKallumLockDto(this KallumLock kallumLock)
        {
            return new KallumLockDto
            {
                SecurePin = kallumLock.SecurePin,
                TransactionPin = kallumLock.TransactionPin

            };
        }
    }
}