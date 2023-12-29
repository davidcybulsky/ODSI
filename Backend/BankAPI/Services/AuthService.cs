using System.Security.Claims;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiContext _apiContext;
        private readonly IHttpContextService _httpContextService;

        public AuthService(ApiContext apiContext,
                           IHttpContextService httpContextService)
        {
            _apiContext = apiContext;
            _httpContextService = httpContextService;
        }

        public async Task<MaskInfoDto> GetMask(GetMaskDto getMaskDto)
        {
            Random random = new Random();
            var user = await _apiContext.Users
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

            await _apiContext.SaveChangesAsync();

            return new MaskInfoDto()
            {
                Mask = maskCode
            };
        }

        public async Task LoginAsync(LoginDto loginDto)
        {
            await Task.Delay(1000);
            var user = await _apiContext.Users
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
                await _apiContext.SaveChangesAsync();
                throw new BadRequestException("Bad login or password");
            }

            string sessionId = Guid.NewGuid().ToString();

            user.SessionTokens.Add(new SessionToken
            {
                Token = sessionId,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            });

            user.CurrentTriesAmmount = 0;

            await _apiContext.SaveChangesAsync();

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

        public Task LogoutAsync()
        {
            _httpContextService.LogoutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Task.CompletedTask;
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

            var userInDb = _apiContext.Users.FirstOrDefault(x => x.Login == signUpDto.Login);

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
            await _apiContext.AddAsync(user);
            await _apiContext.SaveChangesAsync();

            return;
        }
    }
}