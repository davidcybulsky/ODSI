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

            var sId = await _httpContextService.GetSessionId();

            var sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId) ?? throw new UnauthorizedException();

            if (sessionInDb.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedException();
            }

            var user = await _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            var result = BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash);

            if (result == false)
            {
                return;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

            user.PasswordHash = hashedPassword;

            user.PartialPasswords.Clear();

            Random random = new Random();

            while (user.PartialPasswords.Count < 10)
            {
                string partialPassword = "";
                List<int> mask = new List<int>();
                while (mask.Count < 8)
                {
                    var position = random.Next(changePasswordDto.NewPassword.Length);
                    if (!mask.Contains(position))
                    {
                        mask.Add(position);
                    }
                }
                mask.Sort();
                foreach (var i in mask)
                {
                    partialPassword += changePasswordDto.NewPassword[i];
                }

                var partialPasswordHash = BCrypt.Net.BCrypt.HashPassword(partialPassword);
                user.PartialPasswords.Add(new PartialPassword()
                {
                    Hash = partialPasswordHash,
                    Mask = mask
                });
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
        }

        public async Task<AccountDto> GetAccountInfoAsync()
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
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            var account = _mapper.Map<AccountDto>(user.Account);

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

            return account;
        }
    }
}