using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services
{
    public class DebitCardService : IDebitCardService
    {
        private readonly ApiContext _dbContext;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;
        private readonly IDataProtector _dataProtector;

        public DebitCardService(ApiContext dbContext,
            ISessionService sessionService,
            IMapper mapper,
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _sessionService = sessionService;
            _mapper = mapper;
            _dataProtector = dataProtectionProvider.CreateProtector(configuration["DataProtector:SymmetricKey"]);
        }

        public async Task<IEnumerable<DebitCardDto>> GetDebitCards()
        {
            string sId = await _sessionService.GetSessionId();

            string csrf = await _sessionService.GetCsrf();

            await _sessionService.Verify(sId, csrf);

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

            await _sessionService.Expire(sId);

            await _sessionService.Create(user, csrf);

            return debitCards;
        }
    }
}