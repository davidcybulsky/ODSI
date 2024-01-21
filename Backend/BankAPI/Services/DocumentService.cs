using AutoMapper;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Exceptions;
using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankAPI.Services;

public class DocumentService : IDocumentService
{
    private readonly ApiContext _dbContext;
    private readonly IHttpContextService _httpContextService;
    private readonly IDataProtector _dataProtector;
    private readonly IMapper _mapper;

    public DocumentService(ApiContext dbContext,
                           IMapper mapper,
                           IHttpContextService httpContextService,
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextService = httpContextService;
        _dataProtector = dataProtectionProvider.CreateProtector(configuration["DataProtector:SymmetricKey"]);
    }

    public async Task<DocumentDto> GetDocumentAsync()
    {
        string sId = await _httpContextService.GetSessionId();

        Data.Entities.SessionToken sessionInDb = await _dbContext.SessionTokens.FirstOrDefaultAsync(x => x.Token == sId) ?? throw new UnauthorizedException();

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

        Data.Entities.User user = _dbContext.Users
            .Include(x => x.SessionTokens)
            .Include(x => x.Account)
            .ThenInclude(x => x.Document)
            .FirstOrDefault(x => x.SessionTokens.
                    Any(s => s.Token == sId)) ?? throw new UnauthorizedException();

        DocumentDto document = _mapper.Map<DocumentDto>(user.Account.Document);

        document.DocumentsNumber = _dataProtector.Unprotect(document.DocumentsNumber);

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


        return document;
    }
}