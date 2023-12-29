using System.Security.Claims;
using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services
{
    public class TransferService : ITransferService
    {
        private readonly ApiContext _dbContext;
        private readonly IHttpContextService _httpContextService;
        private readonly IMapper _mapper;

        public TransferService(ApiContext dbContext,
            IHttpContextService httpContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextService = httpContextService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransferDto>> GetPaymentHistoryAsync()
        {
            var sId = await _httpContextService.GetSessionId();

            var sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId) ?? throw new UnauthorizedException();

            if (sessionInDb.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedException();
            }

            var user = _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .ThenInclude(x => x.Transfers)
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            List<TransferDto> transferDtos = [];

            var receivedTransfers = _dbContext.Transfers.Where(x => x.ReceiverId == user.Id).ToList();

            foreach (var transfer in receivedTransfers)
            {
                var dto = _mapper.Map<TransferDto>(transfer);
                dto.BalanceBefore = transfer.ReceiversBalanceBefore;
                dto.BalanceAfter = transfer.ReceiversBalanceAfter;
                transferDtos.Add(dto);
            }

            var issuedTransfers = _dbContext.Transfers.Where(x => x.IssuerId == user.Id).ToList();

            foreach (var transfer in issuedTransfers)
            {
                var dto = _mapper.Map<TransferDto>(transfer);
                dto.BalanceBefore = transfer.IssuersBalanceBefore;
                dto.BalanceAfter = transfer.IssuersBalanceAfter;
                transferDtos.Add(dto);
            }

            var token = _dbContext.SessionTokens.FirstOrDefault(x => x.Token == sId)!;

            token.ExpirationDate = DateTime.UtcNow;

            string sessionId = Guid.NewGuid().ToString();

            user.SessionTokens.Add(new SessionToken
            {
                Token = sessionId,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            });

            await _dbContext.SaveChangesAsync();

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, sessionId)
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                ));

            await _httpContextService.AuthenticateUsingCookie(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return transferDtos;
        }

        public async Task MakePaymentAsync(MakeTransferDto makePaymentDto)
        {
            var sId = await _httpContextService.GetSessionId();

            var sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId) ?? throw new UnauthorizedException();

            if (sessionInDb.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedException();
            }

            var issuer = _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .ThenInclude(x => x.Transfers)
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            var receiver = _dbContext.Users
                .Include(x => x.Account)
                .ThenInclude(x => x.Transfers)
                .FirstOrDefault(x => x.Account.AccountNumber == makePaymentDto.ReceiversAccountNumber)
                    ?? throw new BadRequestException();

            if (issuer.Account.AmountOfMoney - makePaymentDto.AmountOfMoney < 0)
            {
                throw new BadRequestException();
            }

            Transfer transfer = new Transfer()
            {
                IssuerId = issuer.Id,
                ReceiverId = receiver.Id,
                IssuersAccountNumber = issuer.Account.AccountNumber,
                ReceiversAccountNumber = receiver.Account.AccountNumber,
                AmountOfMoney = makePaymentDto.AmountOfMoney,
                Title = makePaymentDto.Title,
                ReceiversBalanceBefore = receiver.Account.AmountOfMoney,
                IssuersBalanceBefore = issuer.Account.AmountOfMoney,
            };

            await _dbContext.AddAsync(transfer);

            receiver.Account.AmountOfMoney += makePaymentDto.AmountOfMoney;
            issuer.Account.AmountOfMoney -= makePaymentDto.AmountOfMoney;

            transfer.ReceiversBalanceAfter = receiver.Account.AmountOfMoney;
            transfer.IssuersBalanceAfter = issuer.Account.AmountOfMoney;

            var token = _dbContext.SessionTokens.FirstOrDefault(x => x.Token == sId)!;

            token.ExpirationDate = DateTime.UtcNow;

            string sessionId = Guid.NewGuid().ToString();

            issuer.SessionTokens.Add(new SessionToken
            {
                Token = sessionId,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            });

            await _dbContext.SaveChangesAsync();

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, sessionId)
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                ));

            await _httpContextService.AuthenticateUsingCookie(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }
}