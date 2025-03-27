using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.DTOs;
using MinimalApi.Models;

namespace MinimalApi.Endpoints;

public static class UserEndpoint
{
    public static void UserEndpoints(this WebApplication app)
    {
        var usersEndpoint = app.MapGroup("users").WithTags("Users").WithOpenApi();
        
        usersEndpoint.MapGet("", async (AppDbContext db) =>
            {
                var users = await db.Users
                    .Select(user => new UserDto(user.Id, user.Name, user.Age))
                    .ToListAsync();

                return Results.Ok(new { Users = users });
            })
            .Produces<UserListResponseDto>()
            .WithSummary("Read Users");

        usersEndpoint.MapGet("/transactions", 
                async (AppDbContext db) =>
            {
                var users = await db.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Age,
                        Transactions = u.Transactions.Select(t => new
                        {
                            t.Id,
                            t.Description,
                            t.Value,
                            Operation = t.Operation.ToString().ToLower(),
                            User_id = t.UserId
                        }).ToList()
                    })
                    .ToListAsync();

                return Results.Ok( new { Users = users });
            })
            .Produces<UserListResponseWithTransactionsDto>(statusCode: StatusCodes.Status200OK)
            .WithSummary("Read Users With Transactions");

        usersEndpoint.MapPost("",
                async (UserRequestDto request, AppDbContext db) =>
            {
                const int minAgeAccept = 1;
                const int maxAgeAccept = 150;

                if (request.Age is > maxAgeAccept or < minAgeAccept)
                {
                    return Results.BadRequest(new Message("User age must be between 1 and 150."));
                }

                var newUser = new User(request.Name, request.Age);
      
                await db.Users.AddAsync(newUser);
                await db.SaveChangesAsync();
                
                var userDto = new UserDto(newUser.Id, newUser.Name, newUser.Age);

                return Results.Created($"/users/{newUser.Id}", userDto);
            })
            .Produces<UserDto>(statusCode: StatusCodes.Status201Created)
            .WithSummary("Create user");
        
        usersEndpoint.MapPut("{id:int}", 
            async (int id, UserRequestDto request, AppDbContext db) =>
            {
                const int minAgeAccept = 1;
                const int maxAgeAccept = 150;

                if (request.Age is > maxAgeAccept or < minAgeAccept)
                {
                    return Results.BadRequest(new Message ("User age must be between 1 and 150."));
                }

                var user = await db.Users.FindAsync(id);
                if (user == null)
                {
                    return Results.NotFound(new Message ("User not found."));
                }
            
                user.Name = request.Name;
                user.Age = request.Age;
            
                await db.SaveChangesAsync();
            
                return Results.Ok(new UserDto(user.Id, user.Name, user.Age));
            })
            .Produces<UserDto>(statusCode: StatusCodes.Status200OK)
            .WithSummary("Update User");
        
        usersEndpoint.MapDelete("{id:int}",
            async (int id, AppDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound(new Message ("User not found."));
                }

                db.Users.Remove(user);
                
                await db.SaveChangesAsync();
                
                return Results.Ok(new Message ("User successfully deleted."));
            })
            .Produces<Message>(statusCode: StatusCodes.Status200OK)
            .WithSummary("Delete User");
    }
}