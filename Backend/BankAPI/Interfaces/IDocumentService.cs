using BankAPI.Models;

namespace BankAPI.Interfaces;

public interface IDocumentService 
{
    Task<DocumentDto> GetDocumentAsync();
}