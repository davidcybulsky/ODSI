using BankAPI.Data;
using BankAPI.Data.Entities;
using Microsoft.AspNetCore.DataProtection;

namespace BankAPI.Services
{
    public class SeedService
    {
        private readonly ApiContext _apiContext;
        private readonly IDataProtector _dataProtector;

        public SeedService(ApiContext apiContext, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _apiContext = apiContext;
            _dataProtector = dataProtectionProvider.CreateProtector(configuration["DataProtector:SymmetricKey"]);
        }

        public void Seed()
        {
            _apiContext.AddRange(
                new User()
                {
                    Id = 1,
                    Login = "user12345678",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("sup3r5tr0ngp4ssw0rd"),
                    PartialPasswords =
                    [
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("sup3r5tr"),
                            Mask = [0, 1, 2, 3, 4, 5, 6, 7],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("up3r5tr0"),
                            Mask = [1, 2, 3, 4, 5, 6, 7, 8],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("p3r5tr0n"),
                            Mask = [2, 3, 4, 5, 6, 7, 8, 9],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("3r5tr0ng"),
                            Mask = [3, 4, 5, 6, 7, 8, 9, 10],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("r5tr0ngp"),
                            Mask = [4, 5, 6, 7, 8, 9, 10, 11],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("5tr0ngp4"),
                            Mask = [5, 6, 7, 8, 9, 10, 11, 12],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("tr0ngp4s"),
                            Mask = [6, 7, 8, 9, 10, 11, 12, 13],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("r0ngp4ss"),
                            Mask = [7, 8, 9, 10, 11, 12, 13, 14],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("0ngp4ssw"),
                            Mask = [8, 9, 10, 11, 12, 13, 14, 15],
                            UserId = 1
                        },
                        new PartialPassword()
                        {
                            Hash = BCrypt.Net.BCrypt.HashPassword("ngp4ssw0"),
                            Mask = [9, 10, 11, 12, 13, 14, 15, 16],
                            UserId = 1
                        }
                    ],
                    Account = new()
                    {
                        Id = 1,
                        UserId = 1,
                        AccountNumber = "1234567890123456789012345678901234",
                        AmountOfMoney = 10000,
                        DebitCards =
                        [
                            new DebitCard()
                            {
                                CardNumber = _dataProtector.Protect("1234567890123456"),
                                AccountId = 1,
                            },
                            new DebitCard()
                            {
                                CardNumber = _dataProtector.Protect("2345678901234567"),
                                AccountId = 1
                            }
                        ],
                        Document = new()
                        {
                            Id = 1,
                            AccountId = 1,
                            FirstName = "Janusz",
                            LastName = "Kowalski",
                            DocumentsNumber = _dataProtector.Protect("ABC123456")
                        }
                    }
                },
                new User()
                {
                    Id = 2,
                    Login = "user11112222",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("sup3rtajnehas1o123"),
                    PartialPasswords =
                    [
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [0, 1, 2, 3, 4, 5, 6, 7],
                            Hash = BCrypt.Net.BCrypt.HashPassword("sup3rtaj")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [1, 2, 3, 4, 5, 6, 7, 8],
                            Hash = BCrypt.Net.BCrypt.HashPassword("up3rtajn")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [2, 3, 4, 5, 6, 7, 8, 9],
                            Hash = BCrypt.Net.BCrypt.HashPassword("p3rtajne")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [3, 4, 5, 6, 7, 8, 9, 10],
                            Hash = BCrypt.Net.BCrypt.HashPassword("3rtajneh")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [4, 5, 6, 7, 8, 9, 10, 11],
                            Hash = BCrypt.Net.BCrypt.HashPassword("rtajneha")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [5, 6, 7, 8, 9, 10, 11, 12],
                            Hash = BCrypt.Net.BCrypt.HashPassword("tajnehas")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [6, 7, 8, 9, 10, 11, 12, 13],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ajnehas1")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [7, 8, 9, 10, 11, 12, 13, 14],
                            Hash = BCrypt.Net.BCrypt.HashPassword("jnehas1o")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [8, 9, 10, 11, 12, 13, 14, 15],
                            Hash = BCrypt.Net.BCrypt.HashPassword("nehas1o1")
                        },
                        new PartialPassword()
                        {
                            UserId = 2,
                            Mask = [9, 10, 11, 12, 13, 14, 15, 16],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ehas1o12")
                        },
                    ],
                    Account = new()
                    {
                        Id = 2,
                        UserId = 2,
                        AccountNumber = "2345678901234567890123456789012345",
                        AmountOfMoney = 10000,
                        DebitCards =
                        [
                            new DebitCard()
                            {
                                CardNumber = _dataProtector.Protect("1234567887654321"),
                                AccountId = 2
                            },
                            new DebitCard()
                            {
                                AccountId = 2,
                                CardNumber = _dataProtector.Protect("7890098778900987")
                            }
                        ],
                        Document = new()
                        {
                            Id = 2,
                            FirstName = "Jan",
                            LastName = "Nowak",
                            AccountId = 2,
                            DocumentsNumber = _dataProtector.Protect("DEF234098")
                        }
                    }
                },
                new User()
                {
                    Id = 3,
                    Login = "user12121212",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("_security_1s_my_wh0le_lov3"),
                    PartialPasswords =
                    [
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [0, 1, 2, 3, 4, 5, 6, 7],
                            Hash = BCrypt.Net.BCrypt.HashPassword("_securit"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [1, 2, 3, 4, 5, 6, 7, 8],
                            Hash = BCrypt.Net.BCrypt.HashPassword("security"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [2, 3, 4, 5, 6, 7, 8, 9],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ecurity_"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [3, 4, 5, 6, 7, 8, 9, 10],
                            Hash = BCrypt.Net.BCrypt.HashPassword("curity_1"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [4, 5, 6, 7, 8, 9, 10, 11],
                            Hash = BCrypt.Net.BCrypt.HashPassword("urity_1s"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [5, 6, 7, 8, 9, 10, 11, 12],
                            Hash = BCrypt.Net.BCrypt.HashPassword("rity_1s_"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [6, 7, 8, 9, 10, 11, 12, 13],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ity_1s_m"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [7, 8, 9, 10, 11, 12, 13, 14],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ty_1s_my"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [8, 9, 10, 11, 12, 13, 14, 15],
                            Hash = BCrypt.Net.BCrypt.HashPassword("y_1s_my_"),
                        },
                        new PartialPassword()
                        {
                            UserId = 3,
                            Mask = [9, 10, 11, 12, 13, 14, 15, 16],
                            Hash = BCrypt.Net.BCrypt.HashPassword("_1s_my_w"),
                        },
                    ],
                    Account = new()
                    {
                        Id = 3,
                        UserId = 3,
                        AccountNumber = "3456789012345678901234567890123456",
                        AmountOfMoney = 200000,
                        Document = new Document()
                        {
                            Id = 3,
                            AccountId = 3,
                            FirstName = "Paweł",
                            LastName = "Nowak",
                            DocumentsNumber = _dataProtector.Protect("LDZ214365")
                        }
                    }
                },
                new User()
                {
                    Id = 4,
                    Login = "user09128734",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("!123admin890qwerty!"),
                    PartialPasswords =
                    [
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [0, 1, 2, 3, 4, 5, 6, 7],
                            Hash = BCrypt.Net.BCrypt.HashPassword("!123admi"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [1, 2, 3, 4, 5, 6, 7, 8],
                            Hash = BCrypt.Net.BCrypt.HashPassword("123admin"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [2, 3, 4, 5, 6, 7, 8, 9],
                            Hash = BCrypt.Net.BCrypt.HashPassword("23admin8"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [3, 4, 5, 6, 7, 8, 9, 10],
                            Hash = BCrypt.Net.BCrypt.HashPassword("3admin89"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [4, 5, 6, 7, 8, 9, 10, 11],
                            Hash = BCrypt.Net.BCrypt.HashPassword("admin890"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [5, 6, 7, 8, 9, 10, 11, 12],
                            Hash = BCrypt.Net.BCrypt.HashPassword("dmin890q"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [6, 7, 8, 9, 10, 11, 12, 13],
                            Hash = BCrypt.Net.BCrypt.HashPassword("min890qw"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [7, 8, 9, 10, 11, 12, 13, 14],
                            Hash = BCrypt.Net.BCrypt.HashPassword("in890qwe"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [8, 9, 10, 11, 12, 13, 14, 15],
                            Hash = BCrypt.Net.BCrypt.HashPassword("n890qwer"),
                        },
                        new PartialPassword()
                        {
                            UserId = 4,
                            Mask = [9, 10, 11, 12, 13, 14, 15, 16],
                            Hash = BCrypt.Net.BCrypt.HashPassword("890qwert"),
                        },

                    ],
                    Account = new()
                    {
                        Id = 4,
                        UserId = 4,
                        AccountNumber = "456790123456789012345678901234567",
                        AmountOfMoney = 100,
                        Document = new()
                        {
                            Id = 4,
                            FirstName = "Władyslaw",
                            LastName = "Zając",
                            AccountId = 4,
                            DocumentsNumber = _dataProtector.Protect("WRE341209")
                        }
                    }
                },
                new User()
                {
                    Id = 5,
                    Login = "user67584912",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("CaptureTheFlagIsMyLife"),
                    PartialPasswords =
                    [
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [0, 1, 2, 3, 4, 5, 6, 7],
                            Hash = BCrypt.Net.BCrypt.HashPassword("CaptureT"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [1, 2, 3, 4, 5, 6, 7, 8],
                            Hash = BCrypt.Net.BCrypt.HashPassword("aptureTh"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [2, 3, 4, 5, 6, 7, 8, 9],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ptureThe"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [3, 4, 5, 6, 7, 8, 9, 10],
                            Hash = BCrypt.Net.BCrypt.HashPassword("tureTheF"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [4, 5, 6, 7, 8, 9, 10, 11],
                            Hash = BCrypt.Net.BCrypt.HashPassword("ureTheFl"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [5, 6, 7, 8, 9, 10, 11, 12],
                            Hash = BCrypt.Net.BCrypt.HashPassword("reTheFla"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [6, 7, 8, 9, 10, 11, 12, 13],
                            Hash = BCrypt.Net.BCrypt.HashPassword("eTheFlag"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [7, 8, 9, 10, 11, 12, 13, 14],
                            Hash = BCrypt.Net.BCrypt.HashPassword("TheFlagI"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [8, 9, 10, 11, 12, 13, 14, 15],
                            Hash = BCrypt.Net.BCrypt.HashPassword("heFlagIs"),
                        },
                        new PartialPassword()
                        {
                            UserId = 5,
                            Mask = [9, 10, 11, 12, 13, 14, 15, 16],
                            Hash = BCrypt.Net.BCrypt.HashPassword("eFlagIsM"),
                        },
                    ],
                    Account = new()
                    {
                        Id = 5,
                        UserId = 5,
                        AccountNumber = "567901234567890123456789012345678",
                        AmountOfMoney = 10000,
                        Document = new()
                        {
                            Id = 5,
                            FirstName = "Tomasz",
                            LastName = "Bąk",
                            AccountId = 5,
                            DocumentsNumber = _dataProtector.Protect("AZA903856")
                        }
                    }
                },
                new User()
                {
                    Id = 6,
                    Login = "user54798431",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdmIn123PaSSw00rd"),
                    PartialPasswords =
                    [
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [0, 1, 2, 3, 4, 5, 6, 7],
                            Hash = BCrypt.Net.BCrypt.HashPassword("AdmIn123"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [1, 2, 3, 4, 5, 6, 7, 8],
                            Hash = BCrypt.Net.BCrypt.HashPassword("dmIn123P"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [2, 3, 4, 5, 6, 7, 8, 9],
                            Hash = BCrypt.Net.BCrypt.HashPassword("mIn123Pa"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [3, 4, 5, 6, 7, 8, 9, 10],
                            Hash = BCrypt.Net.BCrypt.HashPassword("In123PaS"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [4, 5, 6, 7, 8, 9, 10, 11],
                            Hash = BCrypt.Net.BCrypt.HashPassword("n123PaSS"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [5, 6, 7, 8, 9, 10, 11, 12],
                            Hash = BCrypt.Net.BCrypt.HashPassword("123PaSSw"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [6, 7, 8, 9, 10, 11, 12, 13],
                            Hash = BCrypt.Net.BCrypt.HashPassword("23PaSSw0"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [7, 8, 9, 10, 11, 12, 13, 14],
                            Hash = BCrypt.Net.BCrypt.HashPassword("3PaSSw00"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [8, 9, 10, 11, 12, 13, 14, 15],
                            Hash = BCrypt.Net.BCrypt.HashPassword("PaSSw00r"),
                        },
                        new PartialPassword()
                        {
                            UserId = 6,
                            Mask = [9, 10, 11, 12, 13, 14, 15, 16],
                            Hash = BCrypt.Net.BCrypt.HashPassword("aSSw00rd"),
                        }
                    ],
                    Account = new()
                    {
                        Id = 6,
                        UserId = 6,
                        AccountNumber = "679012345678901234567890123456789",
                        AmountOfMoney = 30000,
                        Document = new()
                        {
                            Id = 6,
                            FirstName = "Adam",
                            LastName = "Polak",
                            AccountId = 6,
                            DocumentsNumber = _dataProtector.Protect("BRO221103")
                        }
                    }
                });
            _apiContext.SaveChanges();
        }
    }
}
