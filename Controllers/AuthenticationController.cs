using System;
using Kallum.DTOS;
using Kallum.Interfaces;
using Kallum.Models;
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
        public async Task<IActionResult> Login([FromBody]LoginDto loginData)
        {
        try    {if (!ModelState.IsValid) return BadRequest(ModelState);
            var loginResult = await _registerRepository.LoginUserAsync(loginData);
            if (loginResult == null) return BadRequest("Username is incorrect");
                return Ok(loginResult);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
