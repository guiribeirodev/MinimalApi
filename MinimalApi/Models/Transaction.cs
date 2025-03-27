namespace MinimalApi.Models;

public class Transaction
{
    public int Id { get; init; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public OperationType Operation { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public Transaction(string description, decimal value, OperationType operation, int userId)
    {
        Description = description;
        Value = value;
        Operation = operation;
        UserId = userId;
    }
}