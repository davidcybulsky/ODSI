namespace BankAPI.Models
{
    public class MakeTransferDto
    {
        public string Title { get; set; } = string.Empty;
        public string ReceiversAccountNumber { get; set; } = string.Empty;
        public double AmountOfMoney { get; set; }
    }
}