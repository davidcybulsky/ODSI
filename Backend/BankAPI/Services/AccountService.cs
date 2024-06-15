using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApiContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISessionService _sessionService;

        public AccountService(
            ApiContext dbContext,
            IMapper mapper,
            ISessionService sessionService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _sessionService = sessionService;
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {

            string sId = await _sessionService.GetSessionId();

            string csrf = await _sessionService.GetCsrf();

            await _sessionService.Verify(sId, csrf);

            ValidateNewPassword(changePasswordDto);

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

            double entropy = CountEntropy(changePasswordDto.NewPassword);

            if (entropy < 60)
            {
                throw new BadRequestException($"Your password is too weak");
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

            await _sessionService.Expire(sId);
        }

        public async Task<AccountDto> GetAccountInfoAsync()
        {
            string sId = await _sessionService.GetSessionId();

            string csrf = await _sessionService.GetCsrf();

            await _sessionService.Verify(sId, csrf);

            User user = _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            AccountDto account = _mapper.Map<AccountDto>(user.Account);

            await _sessionService.Expire(sId);

            await _sessionService.Create(user, csrf);

            return account;
        }

        private double CountEntropy(string password)
        {

            int L = password.Length;
            int R = 0;
            if (password.Any(x => char.IsUpper(x)))
            {
                R += 26;
            }
            if (password.Any(x => char.IsLower(x)))
            {
                R += 26;
            }
            if (password.Any(x => char.IsNumber(x)))
            {
                R += 10;
            }

            string withoutSpecial = new(password.Where(c => Char.IsLetterOrDigit(c)).ToArray());

            if (withoutSpecial != password)
            {
                R += 30;
            }

            double entropy = Math.Log2(Math.Pow(R, L));

            return entropy;
        }

        private void ValidateNewPassword(ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword == string.Empty || changePasswordDto.ConfirmedPassword == string.Empty || changePasswordDto.CurrentPassword == string.Empty)
            {
                throw new BadRequestException("All fields are required");
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmedPassword)
            {
                throw new BadRequestException("Passwords are diffferent");
            }

            if (changePasswordDto.NewPassword.Length < 12)
            {
                throw new BadRequestException("New password is too short");
            }
        }
    }
}