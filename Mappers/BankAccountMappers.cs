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
                BankAccountId = bankAccount.BankAccountId,
                KallumUser = new AppUserDto
                {
                    Email = bankAccount.AppUser.Email,
                    UserName = bankAccount.AppUser.UserName
                }

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

        public static AppUserDto ToAppUserDto(this UserAccount userAccount)
        {
            return new AppUserDto
            {
                Email = userAccount.Email,
                UserName = userAccount.FullName
            };
        }
    }
}