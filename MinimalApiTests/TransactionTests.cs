using System.Net;
using System.Net.Http.Json;
using MinimalApiTests.Helpers;

namespace MinimalApiTests;
public class TransactionApiIntegrationTests : IntegrationTestBase<Program>, IClassFixture<TransactionApiIntegrationTests>
{
    [Fact]
    public async Task Get_ListAllTransactions()
    {
        //Arrange
        const string expected = """{"transactions":[]}""";

        var response = await Client.GetAsync("/transactions");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }

    [Fact]
    public async Task Get_ListAllTransactionsWithTransaction()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        await TransactionCreator.CreateTransactionAsync(
            Client,
            "TestDescription",
            100,
            "expense",
            1
        );        
        const string expected = """{"transactions":[{"id":1,"description":"TestDescription","value":100.0,"operation":"expense","user_id":1}]}""";
        
        var response = await Client.GetAsync("/transactions");
        var responseContent = await response.Content.ReadAsStringAsync();
      
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Post_CreateTransaction()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        const string expected = """{"id":1,"description":"TestDescription","value":100,"operation":"expense","userId":1}""";
        var newTransaction = new
        {
            Description = "TestDescription", Value = 100, Operation = "expense", User_Id = 1
        };

        var response = await Client.PostAsJsonAsync("/transactions", newTransaction);
        var responseContent = await response.Content.ReadAsStringAsync();
      
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }

    [Fact]
    public async Task Post_CreateTransactionFailNotUser()
    {
        const string expected = """{"message":"User not found."}""";
        var newTransaction = new
        {
            Description = "TestDescription", Value = 100, Operation = "expense", User_Id = 1
        };

        var response = await Client.PostAsJsonAsync("/transactions", newTransaction);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
        
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }

    [Fact]
    public async Task Post_CreateTransactionFailUserUnder18YearsOld()
    {
        const string expected = """{"message":"User is under 18 years old."}""";
        await UserCreator.CreateUserAsync(Client, "João", 17);
        var newTransaction = new
        {
            Description = "TestDescription", Value = 100, Operation = "income", User_Id = 1
        };

        var response = await Client.PostAsJsonAsync("/transactions", newTransaction);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
        
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Post_CreateTransactionFailNegativeValue()
    {
        const string expected = """{"message":"Value must be a positive number."}""";
        await UserCreator.CreateUserAsync(Client, "João", 25);
        var newTransaction = new
        {
            Description = "TestDescription", Value = -100, Operation = "income", User_Id = 1
        };

        var response = await Client.PostAsJsonAsync("/transactions", newTransaction);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
        
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }

    [Fact]
    public async Task Delete_DeleteTransaction()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        await TransactionCreator.CreateTransactionAsync(
            Client,
            "TestDescription",
            100,
            "expense",
            1
        );        
        const string expected = """{"message":"Transaction successfully deleted."}""";
        
        var response = await Client.DeleteAsync("/transactions/1");
        var responseContent = await response.Content.ReadAsStringAsync();
      
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Delete_DeleteTransactionNotFound()
    {
        const string expected = """{"message":"Transaction not found."}""";

        var response = await Client.DeleteAsync("/transactions/1");
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
        
        Assert.Equal(HttpStatusCode.NotFound, responseCode);
        Assert.Equal(expected, responseContent);
    }
}