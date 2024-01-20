using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankAPI.Services
{
    public class DebitCardService : IDebitCardService
    {
        private readonly ApiContext _dbContext;
        private readonly IHttpContextService _httpContextService;
        private readonly IMapper _mapper;
        private readonly IDataProtector _dataProtector;

        public DebitCardService(ApiContext dbContext,
            IHttpContextService httpContextService,
            IMapper mapper,
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpContextService = httpContextService;
            _mapper = mapper;
            _dataProtector = dataProtectionProvider.CreateProtector(configuration["DataProtector:SymmetricKey"]);
        }

        public async Task<IEnumerable<DebitCardDto>> GetDebitCards()
        {
            string sId = await _httpContextService.GetSessionId();

            SessionToken sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId) ?? throw new UnauthorizedException();

            if (sessionInDb.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedException();
            }

            string csrf = await _httpContextService.GetCsrfTokenAsync();

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

            User user = _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .ThenInclude(x => x.DebitCards)
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            IEnumerable<DebitCardDto> debitCards = _mapper.Map<IEnumerable<DebitCardDto>>(user.Account.DebitCards);

            foreach (DebitCardDto debitCard in debitCards)
            {
                debitCard.CardNumber = _dataProtector.Unprotect(debitCard.CardNumber);
            }

            SessionToken token = _dbContext.SessionTokens.FirstOrDefault(x => x.Token == sId)!;

            token.ExpirationDate = DateTime.UtcNow;

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

            return debitCards;
        }
    }
}