using BankAPI.Models;

namespace BankAPI.Interfaces
{
    public interface IAuthService
    {
        Task<MaskInfoDto> GetMask(GetMaskDto getMaskDto);
        Task<CsrfDto> LoginAsync(LoginDto loginDto);
        Task SignUpAsync(SignUpDto signUpDto);
        Task LogoutAsync();
        Task<CsrfDto> IsAuthenticatedAsync();
    }
}