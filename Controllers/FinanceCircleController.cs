using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.FinanceCircle;
using Kallum.Extensions;
using Kallum.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/financecircle")]
    public class FinanceCircleController : ControllerBase
    {
        private readonly IFinanceCircleRepository _financeCircleRepository;
        public FinanceCircleController(IFinanceCircleRepository financeCircleRepository)
        {
            _financeCircleRepository = financeCircleRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCircle([FromBody] CreateFinanceCircleDto createFinance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = JsonConvert.SerializeObject(createFinance, Formatting.Indented);

            var username = User.GetUsername();

            var circleStatus = await _financeCircleRepository.CreateFinanceCircle(createFinance, username);
            if (circleStatus is null)
            {
                return BadRequest();
            }
            return Ok(circleStatus);
        }
        [HttpGet]
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
        [HttpGet("eligible")]
        [Authorize]
        public async Task<IActionResult> CheckEligibility()
        {
            var username = User.GetUsername();
            var IsUserEligible = await _financeCircleRepository.IsUserEligible(username);
            return Ok(IsUserEligible);
        }
    }
}