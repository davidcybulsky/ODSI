using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApiContext _dbContext;
        private readonly IHttpContextService _httpContextService;
        private readonly IMapper _mapper;

        public AccountService(ApiContext dbContext,
            IHttpContextService httpContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextService = httpContextService;
            _mapper = mapper;
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmedPassword)
            {
                throw new BadRequestException("Passwords are diffferent");
            }

            if (changePasswordDto.NewPassword.Length < 12)
            {
                throw new BadRequestException("New password does not fullfil security requiements");
            }

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

            User user = await _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            bool result = BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash);

            if (result == false)
            {
                throw new BadRequestException("Bad password");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

            user.PasswordHash = hashedPassword;

            user.PartialPasswords.Clear();

            Random random = new();

            while (user.PartialPasswords.Count < 10)
            {
                string partialPassword = "";
                List<int> mask = [];
                while (mask.Count < 8)
                {
                    int position = random.Next(changePasswordDto.NewPassword.Length);
                    if (!mask.Contains(position))
                    {
                        mask.Add(position);
                    }
                }
                mask.Sort();
                foreach (int i in mask)
                {
                    partialPassword += changePasswordDto.NewPassword[i];
                }

                string partialPasswordHash = BCrypt.Net.BCrypt.HashPassword(partialPassword);
                user.PartialPasswords.Add(new PartialPassword()
                {
                    Hash = partialPasswordHash,
                    Mask = mask
                });
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
        }

        public async Task<AccountDto> GetAccountInfoAsync()
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
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            AccountDto account = _mapper.Map<AccountDto>(user.Account);

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

            return account;
        }
    }
}