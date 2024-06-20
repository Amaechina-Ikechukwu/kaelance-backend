using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS;
using Kallum.Interfaces;
using Kallum.Models;
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


        public RegisterRepository(ApplicationDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<NewUserDto> RegisterUserAsync(RegisterDto registerData)
        {

            var appUser = new AppUser
            {
                UserName = registerData.FullName.Replace(" ", "_"),
                Email = registerData.Email,
                PhoneNumber = registerData.PhoneNumber
            };
            var createdUser = await _userManager.CreateAsync(appUser, registerData.PassWord);

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (roleResult.Succeeded)
                {
                    return new NewUserDto
                    {
                        UserName = appUser.UserName,
                        Email = appUser.Email,
                        Token = _tokenService.CreateToken(appUser)
                    };
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
        public async Task<NewUserDto> LoginUserAsync(LoginDto loginData)
        {
            Console.WriteLine(loginData.UserName, loginData.PassWord);
            var normailizedName = loginData.UserName.Replace(" ", "_").ToLower();
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.UserName.ToLower() == normailizedName);
            if (user is null) return null;
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
    }
}