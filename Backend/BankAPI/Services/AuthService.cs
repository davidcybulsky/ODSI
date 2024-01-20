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
            Random random = new();
            User? user = await _dbContext.Users
                .Include(x => x.PartialPasswords)
                .FirstOrDefaultAsync(x => x.Login == getMaskDto.Login);

            await Task.Delay(1000);

            if (user == null)
            {
                List<int> dummyMask = [];

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

            List<PartialPassword> passwords = user.PartialPasswords.ToList();
            int index = random.Next(passwords.Count());
            PartialPassword partialPassword = passwords.ToArray()[index];
            List<int> maskCode = partialPassword.Mask;

            user.CurrentPartialPassword = partialPassword.Id;

            await _dbContext.SaveChangesAsync();

            return new MaskInfoDto()
            {
                Mask = maskCode
            };
        }

        public async Task<CsrfDto> IsAuthenticatedAsync()
        {
            string sId = await _httpContextService.GetSessionId();

            User user = await _dbContext.Users.Include(x => x.SessionTokens).FirstOrDefaultAsync(x => x.SessionTokens.Any(x => x.Token == sId)) ?? throw new UnauthorizedException();

            if (user.SessionTokens.FirstOrDefault(x => x.Token == sId)!.ExpirationDate > DateTime.UtcNow)
            {
                string csrf = Guid.NewGuid().ToString();
                SessionToken? session = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId);
                session!.CsrfToken = csrf;
                await _dbContext.SaveChangesAsync();
                return new CsrfDto()
                {
                    Csrf = csrf
                };
            }
            else
            {
                return new CsrfDto();
            }
        }

        public async Task<CsrfDto> LoginAsync(LoginDto loginDto)
        {
            await Task.Delay(1000);
            User? user = await _dbContext.Users
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

            string partialPasswordHash = user.PartialPasswords.FirstOrDefault(x => x.Id == user.CurrentPartialPassword)!.Hash;

            bool result = BCrypt.Net.BCrypt.Verify(loginDto.PartialPassword, partialPasswordHash);

            if (result == false)
            {
                user.CurrentTriesAmmount++;
                await _dbContext.SaveChangesAsync();
                throw new BadRequestException("Bad login or password");
            }

            string sessionId = Guid.NewGuid().ToString();

            string csrf = Guid.NewGuid().ToString();

            user.SessionTokens.Add(new SessionToken
            {
                CsrfToken = csrf,
                Token = sessionId,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            });

            user.CurrentTriesAmmount = 0;

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

            return (new CsrfDto()
            {
                Csrf = csrf
            });
        }

        public async Task LogoutAsync()
        {
            string sId = await _httpContextService.GetSessionId();

            SessionToken token = _dbContext.SessionTokens.FirstOrDefault(x => x.Token == sId);


            string csrf = await _httpContextService.GetCsrfTokenAsync();



            if (token != null)
            {
                if (csrf is null)
                {
                    throw new UnauthorizedException();
                }
                else
                {
                    if (csrf != token.CsrfToken)
                    {
                        throw new UnauthorizedException();
                    }
                }

                if (token.ExpirationDate < DateTime.UtcNow)
                {
                    await _httpContextService.LogoutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

                token.ExpirationDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

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

            User? userInDb = _dbContext.Users.FirstOrDefault(x => x.Login == signUpDto.Login);

            if (userInDb != null)
            {
                throw new BadRequestException("Contact us");
            }

            User user = new()
            {
                Login = signUpDto.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password)
            };

            Random random = new();

            while (user.PartialPasswords.Count < 10)
            {
                string partialPassword = "";
                List<int> mask = [];
                while (mask.Count < 8)
                {
                    int position = random.Next(signUpDto.Password.Length);
                    if (!mask.Contains(position))
                    {
                        mask.Add(position);
                    }
                }
                mask.Sort();
                foreach (int i in mask)
                {
                    partialPassword += signUpDto.Password[i];
                }

                string partialPasswordHash = BCrypt.Net.BCrypt.HashPassword(partialPassword);
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