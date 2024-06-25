using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.Transactions;
using Kallum.Extensions;
using Kallum.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class Transactions : ControllerBase
    {
        private readonly ITransactionsRepository _transactionRepository;
        public Transactions(ITransactionsRepository transactionsRepository)
        {
            _transactionRepository = transactionsRepository;
        }

        [HttpPost("bloataccount")]
        public async Task<IActionResult> BloatAccount([FromBody] BloatAccountDto transactionDetails)
        {
            try
            {
                var transactionResult = await _transactionRepository.BloatAccount(transactionDetails.RecieverId, transactionDetails.Amount);
                if (transactionResult == null)
                {
                    return StatusCode(500, "Could not process this transaction at the moment");
                }
                return Ok(transactionResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        [HttpPost("sendMoney")]
        [Authorize]
        public async Task<IActionResult> SendMoney([FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var username = User.GetUsername();
                var transactionResult = await _transactionRepository.SendMoney(transactionDto, username);

                if (transactionResult == null)
                {
                    return StatusCode(500, "Could not process this transaction at the moment");
                }
                return Ok(transactionResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet("transactions")]
        [Authorize]
        public async Task<IActionResult> GetTransactionHistory()
        {
            try
            {
                var username = User.GetUsername();
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username is null or empty.");
                }

                var transactionResult = await _transactionRepository.GetTransactionHistory(username);

                if (transactionResult == null)
                {
                    // Log the issue for further diagnosis
                    return NotFound("No transaction history found.");
                }

                return Ok(transactionResult);
            }
            catch (Exception e)
            {
                // Log the exception for further diagnosis
                return StatusCode(500, new { message = e.Message, stackTrace = e.StackTrace });
            }
        }


    }
}