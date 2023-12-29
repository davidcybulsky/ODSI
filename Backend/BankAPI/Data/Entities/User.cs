namespace BankAPI.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Account Account { get; set; } = new();
        public List<PartialPassword> PartialPasswords { get; set; } = [];
        public List<SessionToken> SessionTokens { get; set; } = [];

        public int CurrentTriesAmmount { get; set; }
        public int CurrentPartialPassword { get; set; }
    }
}