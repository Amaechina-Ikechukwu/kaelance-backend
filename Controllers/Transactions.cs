using System;
using System.Threading.Tasks;
using Kallum.DTOS.Transactions;
using Kallum.Extensions;
using Kallum.Helper;
using Kallum.Models;
using Kallum.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsRepository _transactionRepository;
        private readonly string? _secretHash;

        public TransactionsController(ITransactionsRepository transactionsRepository, IOptions<FlwSecretOptions> flwSecretOptions)
        {
            _transactionRepository = transactionsRepository;
            _secretHash = flwSecretOptions.Value.SecretHash;
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
                return StatusCode(500, new { message = e.Message, stackTrace = e.StackTrace });
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
                return StatusCode(500, new { message = e.Message, stackTrace = e.StackTrace });
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
                    return NotFound("No transaction history found.");
                }

                return Ok(transactionResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message, stackTrace = e.StackTrace });
            }
        }

        [HttpPost("topup-webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] ChargeCompletedEvent webhookEvent, [FromHeader(Name = "verif-hash")] string signature)
        {
            try
            {
                string webhookEventJson = JsonConvert.SerializeObject(webhookEvent, Formatting.Indented);
                // Console.WriteLine(webhookEventJson);
                if (string.IsNullOrEmpty(signature) || signature != _secretHash)
                {
                    // This request isn't from Flutterwave; discard
                    return Unauthorized();
                }
                // Console.WriteLine("Going");
                var eventResponse = await _transactionRepository.TopUpWeebhook(webhookEvent);
                if (eventResponse == null)
                {
                    return NoContent();
                }
                return Ok(eventResponse);
            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message, stackTrace = e.StackTrace });
            }
        }
    }
}
