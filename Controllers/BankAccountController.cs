using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.Bank;
using Kallum.Extensions;
using Kallum.Interfaces;
using Kallum.Models;
using Kallum.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/bankoperations")]
    public class BankAccountController : ControllerBase
    {
        public readonly ApplicationDBContext _context;
        public readonly UserManager<AppUser> _userManager;
        public readonly IBankOperationRepository _bankRepository;

        public BankAccountController(ApplicationDBContext context, UserManager<AppUser> userManager, IBankOperationRepository bankOperationnRepository)
        {
            _context = context;
            _userManager = userManager;
            _bankRepository = bankOperationnRepository;
        }
        [HttpGet("accountdetails")]
        [Authorize]
        public async Task<IActionResult> BankDetails()
        {
            try
            {
                var username = User.GetUsername();
                var userInfo = await _userManager.FindByNameAsync(username);
                var userId = userInfo.Id;
                if (userId == null) return BadRequest("User not found");

                var bankDetails = await _bankRepository.CreateBankAccount(userId);
                return Ok(bankDetails);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

        }

        [HttpGet("balance")]
        [Authorize]
        public async Task<IActionResult> GetBankDetails()
        {
            try
            {
                var username = User.GetUsername();
                var bankDetailsResult = await _bankRepository.GetBalanceDetails(username);
                if (bankDetailsResult is null)
                {
                    return BadRequest();
                }
                return Ok(bankDetailsResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        [HttpGet("lockcomplete")]
        [Authorize]
        public async Task<IActionResult> CheckIFLockIsSet()
        {
            try
            {
                var username = User.GetUsername();
                var kallumLockStatus = await _bankRepository.GetKallumLockStatus(username);
                if (kallumLockStatus is null)
                {
                    return BadRequest();
                }
                return Ok(kallumLockStatus);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        [HttpPost("kallumlock")]
        [Authorize]
        public async Task<IActionResult> SetKallumLock(KallumLockDto lockdetails)
        {
            try
            {
                var username = User.GetUsername();
                var kallumLockStatus = await _bankRepository.SetKallumLock(username, lockdetails);
                if (kallumLockStatus is null)
                {
                    return BadRequest();
                }
                return Ok(kallumLockStatus);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}