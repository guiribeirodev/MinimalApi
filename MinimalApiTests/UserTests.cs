using System.Net;
using System.Net.Http.Json;
using MinimalApiTests.Helpers;

namespace MinimalApiTests;

public class UserApiIntegrationTests : IntegrationTestBase<Program>, IClassFixture<UserApiIntegrationTests>
{
    [Fact]
    public async Task Get_ReadUsers()
    {
        const string expected = """{"users":[]}""";

        var response = await Client.GetAsync("/users");
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected, responseContent);
    }

    [Fact]
    public async Task Get_ReadUsersWithUser()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        const string expected = """{"users":[{"id":1,"name":"João","age":25}]}""";
    
        var response = await Client.GetAsync("/users");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Get_ReadUsersWithTransactions()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        const string expected = """{"users":[{"id":1,"name":"João","age":25,"transactions":[]}]}""";
    
        var response = await Client.GetAsync("/users/transactions");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Post_CreateUser()
    {
        var newUser = new { Name = "João", Age = 25 };
        const string expected = """{"id":1,"name":"João","age":25}""";
        
        var response = await Client.PostAsJsonAsync("/users", newUser);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Post_CreateUserUnder0YearsOld()
    {
        var newUser = new { Name = "João", Age = 0 };
        const string expected = """{"message":"User age must be between 1 and 150."}""";
    
        var response = await Client.PostAsJsonAsync("/users", newUser);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
        
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Post_CreateUserAbove150YearsOld()
    {
        var newUser = new { Name = "João", Age = 151 };
        const string expected = """{"message":"User age must be between 1 and 150."}""";
    
        var response = await Client.PostAsJsonAsync("/users", newUser);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
        
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Put_UpdateUser()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        const string expected = """{"id":1,"name":"Lucas","age":26}""";
    
        var updatedUser = new { Name = "Lucas", Age = 26 };
        var updateResponse = await Client.PutAsJsonAsync($"/users/1", updatedUser);
        var responseContent = await updateResponse.Content.ReadAsStringAsync();
    
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Put_UpdateUserUnder0YearsOld()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        var updatedUser = new { Name = "Lucas", Age = 0 };
        const string expected = """{"message":"User age must be between 1 and 150."}""";
    
        var response = await Client.PutAsJsonAsync($"/users/1", updatedUser);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
    
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Put_UpdateAbove150YearsOld()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        var updatedUser = new { Name = "Lucas", Age = 151 };
        const string expected = """{"message":"User age must be between 1 and 150."}""";
    
        var response = await Client.PutAsJsonAsync($"/users/1", updatedUser);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
    
        Assert.Equal(HttpStatusCode.BadRequest, responseCode);
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Put_UpdateUserNotFound()
    {
        var newUser = new { Name = "João", Age = 25 };
        const string expected = """{"message":"User not found."}""";
    
        var response = await Client.PutAsJsonAsync("/users/1", newUser);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
    
        Assert.Equal(HttpStatusCode.NotFound, responseCode);
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Delete_RemoveUser()
    {
        await UserCreator.CreateUserAsync(Client, "João", 25);
        const string expected = """{"message":"User successfully deleted."}""";
    
        var response = await Client.DeleteAsync($"/users/1");
        var responseContent = await response.Content.ReadAsStringAsync();
    
        response.EnsureSuccessStatusCode();
        Assert.Equal(expected, responseContent);
    }
    
    [Fact]
    public async Task Delete_RemoveUserNotFound()
    {
        const string expected = """{"message":"User not found."}""";
    
        var response = await Client.DeleteAsync($"/users/1");
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseCode =response.StatusCode;
    
        Assert.Equal(HttpStatusCode.NotFound, responseCode);
        Assert.Equal(expected, responseContent);
    }
}