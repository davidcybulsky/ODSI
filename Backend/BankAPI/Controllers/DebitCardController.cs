using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("card")]
    public class DebitCardController : ControllerBase
    {
        private readonly IDebitCardService _service;

        public DebitCardController(IDebitCardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DebitCardDto>>> GetDebitCardsAsync()
        {
            IEnumerable<DebitCardDto> cards = await _service.GetDebitCards();
            return Ok(cards);
        }
    }
}