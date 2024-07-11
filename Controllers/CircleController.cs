using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Extensions;
using Kallum.Helper;
using Kallum.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/circle")]
    public class CircleController : ControllerBase
    {
        private readonly ICircleRepository _circleRepository;
        public CircleController(ICircleRepository circleRepository)
        {
            _circleRepository = circleRepository;
        }
        [HttpGet("{circleId}")]
        [Authorize]
        public async Task<IActionResult> GetCircleActivity([FromRoute] Guid circleId)
        {
            var circle = await _circleRepository.ActivityHistory(circleId);
            if (circle is null)
            {
                return NotFound();
            }
            return Ok(circle);
        }
        [HttpPatch("withdrawfromcircle/{circleId}")]
        [Authorize]
        public async Task<IActionResult> WithdrawFromCircle([FromRoute] Guid circleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userName = User.GetUsername();
            CircleResponseDto result = await _circleRepository.InitiatePersonalWithdrawalFromCircle(userName, circleId);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("commit/{circleId}")]
        [Authorize]
        public async Task<IActionResult> CommitToCircle([FromRoute] Guid circleId, [FromBody] CommitToCircle commitToCircle)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userName = User.GetUsername();
                CircleResponseDto result = await _circleRepository.CommitToCircle(circleId, userName, commitToCircle.Percentage);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Conflict(new CircleResponseDto { Message = e.Message });
            }
        }
    }
}