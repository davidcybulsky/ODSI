using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankAPI.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApiContext _dbContext;
        private readonly IHttpContextService _httpContextService;

        public SessionService(
            ApiContext dbcontext,
            IHttpContextService httpContextService)
        {
            _dbContext = dbcontext;
            _httpContextService = httpContextService;
        }
        public async Task Create(User user, string csrf)
        {
            string sessionId = Guid.NewGuid().ToString();

            user.SessionTokens.Add(new SessionToken
            {
                Token = sessionId,
                CsrfToken = csrf,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            });

            await _dbContext.SaveChangesAsync();

            ClaimsPrincipal claimsPrincipal = new(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, sessionId)
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                ));

            await _httpContextService.AuthenticateUsingCookie(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        public async Task Expire(string sessionId)
        {
            SessionToken token = _dbContext.SessionTokens.FirstOrDefault(x => x.Token == sessionId)!;

            token.ExpirationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GetCsrf()
        {
            return await _httpContextService.GetCsrfTokenAsync();
        }

        public Task<string> GetSessionId()
        {
            return _httpContextService.GetSessionId();
        }

        public async Task Logout()
        {

            await _httpContextService.LogoutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task Verify(string sessionId, string csrf)
        {
            SessionToken sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sessionId) ?? throw new UnauthorizedException();

            if (sessionInDb.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedException();
            }

            if (csrf is null)
            {
                throw new UnauthorizedException();
            }
            else
            {
                if (csrf != sessionInDb.CsrfToken)
                {
                    throw new UnauthorizedException();
                }
            }
        }
    }
}
