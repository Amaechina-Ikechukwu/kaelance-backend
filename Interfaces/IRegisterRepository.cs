using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS;
using Kallum.Models;

namespace Kallum.Interfaces
{
    public interface IRegisterRepository
    {
        Task<NewUserDto> RegisterUserAsync(RegisterDto registerData);
        Task<NewUserDto> LoginUserAsync(LoginDto loginData);
    }
}