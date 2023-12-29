namespace BankAPI.Data.Entities
{
    public class PartialPassword
    {
        public int Id { get; set; }
        public string Hash { get; set; } = string.Empty;
        public List<int> Mask { get; set; } = [];
        public int UserId { get; set; }
    }
}
