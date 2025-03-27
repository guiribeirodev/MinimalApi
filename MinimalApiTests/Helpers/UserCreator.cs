using System.Net.Http.Json;

namespace MinimalApiTests.Helpers;
public static class UserCreator
{
    public static async Task<HttpResponseMessage> CreateUserAsync(HttpClient client, string name, int age)
    {
        var newUser = new { Name = name, Age = age };
        return await client.PostAsJsonAsync("/users", newUser);
    }
}