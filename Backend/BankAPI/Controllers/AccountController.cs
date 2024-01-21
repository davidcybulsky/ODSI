using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<AccountDto>> GetAccountAsync()
        {
            AccountDto account = await _service.GetAccountInfoAsync();
            return Ok(account);
        }

        [HttpPut]
        public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto)
        {
            await _service.ChangePasswordAsync(changePasswordDto);
            return Ok();
        }
    }
}