using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.FinanceCircle;
using Kallum.Extensions;
using Kallum.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceCircleController : ControllerBase
    {
        private readonly IFinanceCircleRepository _financeCircleRepository;
        public FinanceCircleController(IFinanceCircleRepository financeCircleRepository)
        {
            _financeCircleRepository = financeCircleRepository;
        }

        [HttpPost("financecircle")]
        [Authorize]
        public async Task<IActionResult> CreateCircle([FromBody] CreateFinanceCircleDto createFinance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var circleStatus = await _financeCircleRepository.CreateFinanceCircle(createFinance);
            if (circleStatus is null)
            {
                return BadRequest();
            }
            return Ok(circleStatus);
        }
        [HttpGet("financeCircle")]
        [Authorize]
        public async Task<IActionResult> GetFinanceCircle()
        {
            var username = User.GetUsername();
            var financeCircle = await _financeCircleRepository.AllFinanceCircle(username);
            if (financeCircle is null)
            {
                return NotFound();
            }
            return Ok(financeCircle);
        }
    }
}