using BankAPI.Models;

namespace BankAPI.Interfaces
{
    public interface ITransferService
    {
        Task<IEnumerable<TransferDto>> GetPaymentHistoryAsync();
        Task MakePaymentAsync(MakeTransferDto makePaymentDto);
    }
}