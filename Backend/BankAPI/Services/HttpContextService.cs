using System.Security.Claims;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace BankAPI.Services
{
    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AuthenticateUsingCookie(string cookieName, ClaimsPrincipal claimsPrincipal)
        {
            await _httpContextAccessor.HttpContext!.SignInAsync(cookieName, claimsPrincipal);
        }

        public Task<ClaimsPrincipal> GetClaimsAsync()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext!.User);
        }

        public Task<string> GetSessionId()
        {
            var user = _httpContextAccessor.HttpContext!.User ?? throw new UnauthorizedException();
            var claim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException();
            return Task.FromResult(claim.Value);
        }

        public async Task LogoutAsync(string scheme)
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(scheme);
        }
    }
}