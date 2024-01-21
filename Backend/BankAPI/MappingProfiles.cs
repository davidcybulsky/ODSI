using AutoMapper;
using BankAPI.Data.Entities;
using BankAPI.Models;

namespace BankAPI
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<DebitCard, DebitCardDto>();
            CreateMap<Transfer, TransferDto>();
            CreateMap<Document, DocumentDto>();
            CreateMap<Account, AccountDto>();
        }
    }
}
