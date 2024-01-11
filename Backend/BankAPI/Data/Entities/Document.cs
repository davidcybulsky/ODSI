namespace BankAPI.Data.Entities;

public class Document
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DocumentsNumber { get; set; }
}