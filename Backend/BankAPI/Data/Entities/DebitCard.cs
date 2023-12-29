namespace BankAPI.Data.Entities
{
    public class DebitCard
    {
        public DebitCard()
        {
            Random random = new Random();
            string chars = "1234567890";
            CardNumber = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string CardNumber { get; set; }
    }
}