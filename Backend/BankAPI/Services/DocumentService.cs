using AutoMapper;
using BankAPI.Data;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class DocumentService : IDocumentService
{
    private readonly ApiContext _dbContext;
    private readonly IHttpContextService _httpContextService;
    private readonly IMapper _mapper;

    public DocumentService(ApiContext dbContext,
                           IMapper mapper,
                           IHttpContextService httpContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextService = httpContextService;
    }

    public async Task<DocumentDto> GetDocumentAsync()
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
                .ThenInclude(x => x.Document)
                .FirstOrDefault(x => x.SessionTokens.
                        Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

        var document = _mapper.Map<DocumentDto>(user.Account.Document);

        return document;
    }
}