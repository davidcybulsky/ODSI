namespace BankAPI.Models
{
    public class TransferDto
    {
        public string Title { get; set; } = string.Empty;
        public double AmountOfMoney { get; set; }
        public string ReceiversAccountNumber { get; set; } = string.Empty;
        public string IssuersAccountNumber { get; set; } = string.Empty;
        public double BalanceBefore { get; set; }
        public double BalanceAfter { get; set; }
    }
}