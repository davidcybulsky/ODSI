using AutoMapper;
using BankAPI.Data;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class DocumentService : IDocumentService
{
    private readonly ApiContext _dbContext;
    private readonly ISessionService _sessionService;
    private readonly IDataProtector _dataProtector;
    private readonly IMapper _mapper;

    public DocumentService(
        ApiContext dbContext,
        IMapper mapper,
        ISessionService sessionService,
        IDataProtectionProvider dataProtectionProvider,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _sessionService = sessionService;
        _dataProtector = dataProtectionProvider.CreateProtector(configuration["DataProtector:SymmetricKey"]);
    }

    public async Task<DocumentDto> GetDocumentAsync()
    {
        string sId = await _sessionService.GetSessionId();

        string csrf = await _sessionService.GetCsrf();

        await _sessionService.Verify(sId, csrf);

        Data.Entities.User user = _dbContext.Users
            .Include(x => x.SessionTokens)
            .Include(x => x.Account)
            .ThenInclude(x => x.Document)
            .FirstOrDefault(x => x.SessionTokens.
                    Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

        DocumentDto document = _mapper.Map<DocumentDto>(user.Account.Document);

        document.DocumentsNumber = _dataProtector.Unprotect(document.DocumentsNumber);

        await _sessionService.Expire(sId);

        await _sessionService.Create(user, csrf);

        return document;
    }
}