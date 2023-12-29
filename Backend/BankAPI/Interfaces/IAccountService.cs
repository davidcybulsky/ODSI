using BankAPI.Models;

namespace BankAPI.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto> GetAccountInfoAsync();
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
}