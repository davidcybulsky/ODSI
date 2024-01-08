using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _http;
        private readonly IAuthService _authService;

        public AuthController(IHttpContextAccessor httpContextAccessor, IAuthService authService)
        {
            _http = httpContextAccessor;
            _authService = authService;
        }

        [HttpPost("mask")]
        public async Task<ActionResult<MaskInfoDto>> GetMask(GetMaskDto getMaskDto)
        {
            var maskInfoDto = await _authService.GetMask(getMaskDto);
            return Ok(maskInfoDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync(LoginDto loginDto)
        {
            await _authService.LoginAsync(loginDto);
            return Ok();
        }

        [HttpPost("add/user")]
        public async Task<ActionResult> SignUpAsync(SignUpDto signUpDto)
        {
            await _authService.SignUpAsync(signUpDto);
            return Ok();
        }
    }
}