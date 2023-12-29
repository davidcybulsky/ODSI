namespace BankAPI.Models
{
    public class LoginDto
    {
        public string Login { get; set; } = string.Empty;
        public string PartialPassword { get; set; } = string.Empty;
    }
}