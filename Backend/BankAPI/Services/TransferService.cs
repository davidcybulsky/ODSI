using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services
{
    public class TransferService : ITransferService
    {
        private readonly ApiContext _dbContext;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;

        public TransferService(ApiContext dbContext,
            ISessionService sessionService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _sessionService = sessionService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransferDto>> GetPaymentHistoryAsync()
        {
            string sId = await _sessionService.GetSessionId();

            string csrf = await _sessionService.GetCsrf();

            await _sessionService.Verify(sId, csrf);

            User user = _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .ThenInclude(x => x.Transfers)
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            List<TransferDto> transferDtos = [];

            List<Transfer> receivedTransfers = await _dbContext.Transfers.Where(x => x.ReceiverId == user.Id).ToListAsync();

            foreach (Transfer? transfer in receivedTransfers)
            {
                TransferDto dto = _mapper.Map<TransferDto>(transfer);
                dto.BalanceBefore = transfer.ReceiversBalanceBefore;
                dto.BalanceAfter = transfer.ReceiversBalanceAfter;
                transferDtos.Add(dto);
            }

            List<Transfer> issuedTransfers = await _dbContext.Transfers.Where(x => x.IssuerId == user.Id).ToListAsync();

            foreach (Transfer? transfer in issuedTransfers)
            {
                TransferDto dto = _mapper.Map<TransferDto>(transfer);
                dto.AmountOfMoney = -transfer.AmountOfMoney;
                dto.BalanceBefore = transfer.IssuersBalanceBefore;
                dto.BalanceAfter = transfer.IssuersBalanceAfter;
                transferDtos.Add(dto);
            }

            await _sessionService.Expire(sId);

            await _sessionService.Create(user, csrf);

            return transferDtos;
        }

        public async Task MakePaymentAsync(MakeTransferDto makePaymentDto)
        {
            string sId = await _sessionService.GetSessionId();

            string csrf = await _sessionService.GetCsrf();

            await _sessionService.Verify(sId, csrf);

            if (makePaymentDto.Title == string.Empty || makePaymentDto.ReceiversAccountNumber == string.Empty || makePaymentDto.AmountOfMoney == 0)
            {
                throw new BadRequestException("All fields are requiered");
            }

            User issuer = await _dbContext.Users
                .Include(x => x.SessionTokens)
                .Include(x => x.Account)
                .ThenInclude(x => x.Transfers)
                .FirstOrDefaultAsync(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

            User receiver = await _dbContext.Users
                .Include(x => x.Account)
                .ThenInclude(x => x.Transfers)
                .FirstOrDefaultAsync(x => x.Account.AccountNumber == makePaymentDto.ReceiversAccountNumber)
                    ?? throw new BadRequestException("Bad account number");

            if (issuer.Account.AccountNumber == receiver.Account.AccountNumber)
            {
                throw new BadRequestException("Bad account number");
            }

            if (makePaymentDto.AmountOfMoney <= 0)
            {
                throw new BadRequestException("Too little money on your account");
            }

            if (issuer.Account.AmountOfMoney - makePaymentDto.AmountOfMoney < 0)
            {
                throw new BadRequestException("Too little money on your account");
            }

            Transfer transfer = new()
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

            await _sessionService.Expire(sId);

            await _sessionService.Create(issuer, csrf);
        }
    }
}