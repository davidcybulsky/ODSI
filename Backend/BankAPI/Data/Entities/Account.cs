namespace BankAPI.Data.Entities
{
    public class Account
    {
        public Account()
        {
            AmountOfMoney = 500;
            Random random = new Random();
            string chars = "1234567890";
            AccountNumber = new string(Enumerable.Repeat(chars, 34).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        public double AmountOfMoney { get; set; }
        public List<DebitCard> DebitCards { get; set; } = [];
        public List<Transfer> Transfers { get; set; } = [];
    }
}