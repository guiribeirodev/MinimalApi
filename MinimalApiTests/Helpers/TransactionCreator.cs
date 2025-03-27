using System.Net.Http.Json;

namespace MinimalApiTests.Helpers;

public static class TransactionCreator
{
    public static async Task<HttpResponseMessage> CreateTransactionAsync(
        HttpClient client, 
        string description, 
        decimal value, 
        string operation, 
        int userId)
    {
        var newTransaction = new 
        { 
            Description = description, 
            Value = value, 
            Operation = operation, 
            User_Id = userId 
        };
        
        return await client.PostAsJsonAsync("/transactions", newTransaction);
    }
}