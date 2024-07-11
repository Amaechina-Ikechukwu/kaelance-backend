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
using Kallum.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Repository
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly ITokenService _tokenService;
        public readonly UserIdService _userIdService;


        public RegisterRepository(ApplicationDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, UserIdService userIdService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userIdService = userIdService;
        }

        public async Task<NewUserDto> RegisterUserAsync(RegisterDto registerData)
        {

            var appUser = new AppUser
            {
                UserName = registerData?.FullName?.Replace(" ", "_"),
                Email = registerData?.Email,
                PhoneNumber = registerData?.PhoneNumber,

            };
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var createdUser = await _userManager.CreateAsync(appUser, registerData.PassWord);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (roleResult.Succeeded)
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    return new NewUserDto
                    {
                        UserName = appUser.UserName,
                        Email = appUser.Email,
                        Token = _tokenService.CreateToken(appUser)
                    };
#pragma warning restore CS8601 // Possible null reference assignment.
                }
                else
                {
                    var errorMessage = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                    throw new Exception(errorMessage);
                }
            }
            else
            {
                var errorMessage = string.Join("; ", createdUser.Errors.Select(error => error.Description));
                throw new Exception(errorMessage);
            }
        }
        public async Task<NewUserDto?> LoginUserAsync(LoginDto loginData)
        {
            if (loginData == null || string.IsNullOrEmpty(loginData.UserName) || string.IsNullOrEmpty(loginData.PassWord))
            {
                return null;
            }

            // Check if the input is an email
            var isEmail = IsValidEmail(loginData.UserName);

            string normalizedName;
            if (isEmail)
            {
                normalizedName = loginData.UserName.ToLower();
            }
            else
            {
                normalizedName = loginData.UserName.Replace(" ", "_").ToLower();
            }

            var user = await GetUserByNameOrEmailAsync(normalizedName, isEmail);

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginData.PassWord, false);

            if (!result.Succeeded)
            {
                throw new Exception("Password is incorrect");
            }

            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<AppUser?> GetUserByNameOrEmailAsync(string userNameOrEmail, bool isEmail)
        {
            if (isEmail)
            {
                return await _userManager.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == userNameOrEmail);
            }
            else
            {
                return await _userManager.Users.FirstOrDefaultAsync(user => user.UserName.ToLower() == userNameOrEmail);
            }
        }
        public async Task<KallumLockDto?> GetKallumLockStatus(string username)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                var KallumLock = await _context.KallumLockData.FirstOrDefaultAsync(details => details.AppUserId == userId);

                if (KallumLock is null)
                {
                    return new KallumLockDto
                    {
                        TransactionPin = null,
                        SecurePin = null
                    };
                }
                return new KallumLockDto
                {
                    TransactionPin = KallumLock.TransactionPin,
                    SecurePin = KallumLock.SecurePin
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<string?> SetKallumLock(string username, KallumLockDto lockdetails)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);
                if (lockdetails.SecurePin != null && lockdetails.TransactionPin != null)
                {
                    var kallumLock = new KallumLock
                    {
                        SecurePin = lockdetails.SecurePin,
                        TransactionPin = lockdetails.TransactionPin,
                        AppUserId = userId
                    };
                    await _context.KallumLockData.AddAsync(kallumLock);
                    await _context.SaveChangesAsync();
                    return "Done";
                }

                else { return null; }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public async Task<string> UpdateKallumLock(string username, KallumLockDto lockDetail)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);

                // Find the existing KallumLock for the given userId
                var kallumLock = await _context.KallumLockData.FirstOrDefaultAsync(kl => kl.AppUserId == userId);

                if (kallumLock == null)
                {
                    // Create a new KallumLock if it doesn't exist
                    kallumLock = new KallumLock
                    {
                        AppUserId = userId,
                        TransactionPin = "",
                        SecurePin = ""
                    };
                    _context.KallumLockData.Add(kallumLock);
                }

                if (!string.IsNullOrEmpty(lockDetail.SecurePin) && !string.IsNullOrWhiteSpace(lockDetail.SecurePin) && kallumLock.SecurePin != lockDetail.SecurePin)
                {
                    kallumLock.SecurePin = lockDetail.SecurePin;
                    _context.Entry(kallumLock).Property(kl => kl.SecurePin).IsModified = true;
                }

                if (!string.IsNullOrEmpty(lockDetail.TransactionPin) && !string.IsNullOrWhiteSpace(lockDetail.TransactionPin) && kallumLock.TransactionPin != lockDetail.TransactionPin)
                {
                    kallumLock.TransactionPin = lockDetail.TransactionPin;
                    _context.Entry(kallumLock).Property(kl => kl.TransactionPin).IsModified = true;
                }
                if (!string.IsNullOrEmpty(lockDetail.PushNotificationToken) && !string.IsNullOrWhiteSpace(lockDetail.PushNotificationToken) && kallumLock.PushNotificationToken != lockDetail.PushNotificationToken)
                {
                    kallumLock.PushNotificationToken = lockDetail.PushNotificationToken;
                    _context.Entry(kallumLock).Property(k1 => k1.PushNotificationToken).IsModified = true;
                }
                await _context.SaveChangesAsync();
                return "Done";
            }
            catch
            {
                // Log the exception here if necessary
                throw;
            }
        }


        public async Task<bool> IsSecurePinCorrect(string username, SecurePinRequest SecurePin)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);

                // Find the existing KallumLock for the given userId
                var kallumLock = await _context.KallumLockData.FirstOrDefaultAsync(kl => kl.AppUserId == userId);


                if (kallumLock == null)
                {
                    throw new Exception($"KallumLock not found for userId: {userId}");
                }
                if (kallumLock.SecurePin == SecurePin.SecurePin)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                // Log the exception here if necessary
                throw;
            }
        }

        public async Task<bool> IsTransactionPinCorrect(string username, TransactionPinRequest TransactionPin)
        {
            try
            {
                var userId = await _userIdService.GetUserId(username);

                // Find the existing KallumLock for the given userId
                var kallumLock = await _context.KallumLockData.FirstOrDefaultAsync(kl => kl.AppUserId == userId);


                if (kallumLock == null)
                {
                    throw new Exception($"KallumLock not found for userId: {userId}");
                }
                if (kallumLock.TransactionPin == TransactionPin.TransactionPin)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                // Log the exception here if necessary
                throw;
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var emailAddress = new System.Net.Mail.MailAddress(email);
                return emailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}