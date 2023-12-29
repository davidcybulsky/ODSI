namespace BankAPI.Data.Entities
{
    public class Transfer
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double AmountOfMoney { get; set; }
        public string ReceiversAccountNumber { get; set; } = string.Empty;
        public string IssuersAccountNumber { get; set; } = string.Empty;
        public double ReceiversBalanceBefore { get; set; }
        public double IssuersBalanceBefore { get; set; }
        public double ReceiversBalanceAfter { get; set; }
        public double IssuersBalanceAfter { get; set; }
        public int IssuerId { get; set; }
        public int ReceiverId { get; set; }
    }
}