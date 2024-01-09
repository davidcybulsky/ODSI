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
    public class AuthService : IAuthService
    {
        private readonly ApiContext _dbContext;
        private readonly IHttpContextService _httpContextService;

        public AuthService(ApiContext dbContext,
                           IHttpContextService httpContextService)
        {
            _dbContext = dbContext;
            _httpContextService = httpContextService;
        }

        public async Task<MaskInfoDto> GetMask(GetMaskDto getMaskDto)
        {
            Random random = new Random();
            var user = await _dbContext.Users
                .Include(x => x.PartialPasswords)
                .FirstOrDefaultAsync(x => x.Login == getMaskDto.Login);

            await Task.Delay(1000);

            if (user == null)
            {
                List<int> dummyMask = new();

                while (dummyMask.Count() < 8)
                {
                    int number = (int)random.NextInt64(31);
                    if (!dummyMask.Contains(number))
                    {
                        dummyMask.Add(number);
                    }
                }

                dummyMask.Sort();

                return new MaskInfoDto()
                {
                    Mask = dummyMask
                };
            }

            var passwords = user.PartialPasswords.ToList();
            var index = random.Next(passwords.Count());
            var partialPassword = passwords.ToArray()[index];
            var maskCode = partialPassword.Mask;

            user.CurrentPartialPassword = partialPassword.Id;

            await _dbContext.SaveChangesAsync();

            return new MaskInfoDto()
            {
                Mask = maskCode
            };
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var sId = await _httpContextService.GetSessionId();

            var sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId) ?? throw new UnauthorizedException();

            if (sessionInDb.ExpirationDate < DateTime.UtcNow)
            {
                throw new UnauthorizedException();
            }
            return true;
        }

        public async Task LoginAsync(LoginDto loginDto)
        {
            await Task.Delay(1000);
            var user = await _dbContext.Users
                .Include(x => x.PartialPasswords)
                .Include(x => x.SessionTokens)
                .FirstOrDefaultAsync(x => x.Login == loginDto.Login);

            if (user == null)
            {
                throw new BadRequestException("Bad login or password");
            }

            if (user.CurrentTriesAmmount > 3)
            {
                throw new ForbiddenException();
            }

            var partialPasswordHash = user.PartialPasswords.FirstOrDefault(x => x.Id == user.CurrentPartialPassword)!.Hash;

            var result = BCrypt.Net.BCrypt.Verify(loginDto.PartialPassword, partialPasswordHash);

            if (result == false)
            {
                user.CurrentTriesAmmount++;
                await _dbContext.SaveChangesAsync();
                throw new BadRequestException("Bad login or password");
            }

            string sessionId = Guid.NewGuid().ToString();

            user.SessionTokens.Add(new SessionToken
            {
                Token = sessionId,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            });

            user.CurrentTriesAmmount = 0;

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

        public async Task LogoutAsync()
        {
            var sId = await _httpContextService.GetSessionId();

            var token = _dbContext.SessionTokens.FirstOrDefault(x => x.Token == sId)!;

            token.ExpirationDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await _httpContextService.LogoutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            if (signUpDto.Password != signUpDto.ConfirmPassword)
            {
                throw new BadRequestException("Passwords are different");
            }

            if (signUpDto.Password.Length < 12)
            {
                throw new BadRequestException("Your password does not fullfil security requirements");
            }

            var userInDb = _dbContext.Users.FirstOrDefault(x => x.Login == signUpDto.Login);

            if (userInDb != null)
            {
                throw new BadRequestException("Contact us");
            }

            User user = new()
            {
                Login = signUpDto.Login,
            };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password);

            Random random = new Random();

            while (user.PartialPasswords.Count < 10)
            {
                string partialPassword = "";
                List<int> mask = new List<int>();
                while (mask.Count < 8)
                {
                    var position = random.Next(signUpDto.Password.Length);
                    if (!mask.Contains(position))
                    {
                        mask.Add(position);
                    }
                }
                mask.Sort();
                foreach (var i in mask)
                {
                    partialPassword += signUpDto.Password[i];
                }

                var partialPasswordHash = BCrypt.Net.BCrypt.HashPassword(partialPassword);
                user.PartialPasswords.Add(new PartialPassword()
                {
                    Hash = partialPasswordHash,
                    Mask = mask
                });
            }
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return;
        }
    }
}