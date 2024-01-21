using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _service;

        public TransferController(ITransferService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfersAsync()
        {
            IEnumerable<TransferDto> payments = await _service.GetPaymentHistoryAsync();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<ActionResult> MakeTransferAsync(MakeTransferDto makeTransferDto)
        {
            await _service.MakePaymentAsync(makeTransferDto);
            return Ok();
        }
    }
}