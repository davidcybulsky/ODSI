using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
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

        [HttpGet("logout")]
        public async Task<ActionResult> LogoutAsync()
        {
            await _authService.LogoutAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<bool>> IsAuthenticatedAsync()
        {
            var isAuthenticated = await _authService.IsAuthenticatedAsync();
            return Ok(isAuthenticated);
        }
    }
}