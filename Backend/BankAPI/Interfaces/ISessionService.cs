using BankAPI.Data.Entities;

namespace BankAPI.Interfaces
{
    public interface ISessionService
    {
        Task Expire(string sessionId);
        Task Create(User user, string csrf);
        Task Verify(string sessionId, string csrf);
        Task<string> GetCsrf();
        Task<string> GetSessionId();
        Task Logout();
    }
}
