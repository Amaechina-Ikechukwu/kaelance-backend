using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kallum.DTOS;
using Kallum.DTOS.Bank;
using Kallum.Models;

namespace Kallum.Interfaces
{
    public interface IRegisterRepository
    {
        Task<NewUserDto> RegisterUserAsync(RegisterDto registerData);
        Task<NewUserDto> LoginUserAsync(LoginDto loginData);

        Task<KallumLockDto> GetKallumLockStatus(string username);
        Task<string?> SetKallumLock(string username, KallumLockDto lockDetails);
        Task<string> UpdateKallumLock(string username, KallumLockDto lockDetails);
        Task<bool> IsSecurePinCorrect(string username, SecurePinRequest SecurePin);
        Task<bool> IsTransactionPinCorrect(string username, TransactionPinRequest TransactionPin);
    }
}