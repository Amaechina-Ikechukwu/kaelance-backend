using System;
using Kallum.DTOS;
using Kallum.DTOS.Bank;
using Kallum.Extensions;
using Kallum.Interfaces;
using Kallum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace kallum.Controllers
{


    [ApiController]
    [Route("api/auth")]

    public class AuthenticationController : ControllerBase
    {
        private readonly IRegisterRepository _registerRepository;


        public AuthenticationController(IRegisterRepository registerRepository)
        {

            _registerRepository = registerRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerData)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var registerResult = await _registerRepository.RegisterUserAsync(registerData);

                if (registerResult != null)
                {
                    return Ok(
                        registerResult
                    );
                }
                else
                {
                    return BadRequest("Could not create User");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginData)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var loginResult = await _registerRepository.LoginUserAsync(loginData);
                if (loginResult == null) return BadRequest("Username is incorrect");
                return Ok(loginResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("kallumlock")]
        [Authorize]
        public async Task<IActionResult> CheckIFLockIsSet()
        {
            try
            {
                var username = User.GetUsername();
                var kallumLockStatus = await _registerRepository.GetKallumLockStatus(username);
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
                var kallumLockStatus = await _registerRepository.SetKallumLock(username, lockdetails);
                if (kallumLockStatus == null)
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

        [HttpPatch("kallumlock")]
        [Authorize]
        public async Task<IActionResult> UpdateKallumLock([FromBody] KallumLockDto lockDto)
        {
            try
            {
                var username = User.GetUsername();
                var kallumLockStatus = await _registerRepository.UpdateKallumLock(username, lockDto);
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
        [HttpPost("kallumsecure")]
        [Authorize]
        public async Task<IActionResult> CheckSecurePin([FromBody] SecurePinRequest securepin)
        {
            try
            {
                var username = User.GetUsername();
                var kallumLockStatus = await _registerRepository.IsSecurePinCorrect(username, securepin);

                return Ok(kallumLockStatus);

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        [HttpPost("kallumtransaction")]
        [Authorize]
        public async Task<IActionResult> CheckTransactionPin([FromBody] TransactionPinRequest transactionpin)
        {
            try
            {
                var username = User.GetUsername();
                var kallumLockStatus = await _registerRepository.IsTransactionPinCorrect(username, transactionpin);

                return Ok(kallumLockStatus);

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

    }
}
