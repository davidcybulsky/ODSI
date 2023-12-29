using System.Security.Claims;

namespace BankAPI.Interfaces
{
    public interface IHttpContextService
    {
        Task<string> GetSessionId();
        Task<ClaimsPrincipal> GetClaimsAsync();
        Task AuthenticateUsingCookie(string scheme, ClaimsPrincipal claimsPrincipal);
        Task LogoutAsync(string scheme);
    }
}