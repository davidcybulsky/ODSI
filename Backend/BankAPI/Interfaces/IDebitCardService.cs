using BankAPI.Models;

namespace BankAPI.Interfaces
{
    public interface IDebitCardService
    {
        Task<IEnumerable<DebitCardDto>> GetDebitCards();
    }
}