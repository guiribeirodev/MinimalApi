using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.DTOs;
using MinimalApi.Models;

namespace MinimalApi.Endpoints;

public static class TransactionEndpoint
{
    public static void TransactionEndpoints(this WebApplication app)
    {
        var transactionsEndpoint = app.MapGroup("transactions").WithTags("Transactions").WithOpenApi();

        transactionsEndpoint.MapGet("", async (AppDbContext db) =>
        {
            var transactions = await db.Transactions
                .Select(transaction => new TransactionDto(
                    transaction.Id, 
                    transaction.Description,
                    transaction.Value, 
                    transaction.Operation, 
                    transaction.UserId))
                .ToListAsync();
            
            return Results.Ok(new { Transactions = transactions });
        })
        .Produces<TransactionListResponseDto>()
        .WithSummary("List All Transactions");

        transactionsEndpoint.MapPost("",
            async (TransactionRequestDto request, AppDbContext db) =>
        {
            if (request.Value < 0)
            {
                return Results.BadRequest(new Message("Value must be a positive number."));
            }
            
            var user = await db.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return Results.BadRequest(new Message("User not found."));
            }
            
            var newTransaction = new Transaction(request.Description, request.Value,request.Operation, request.UserId);
        
            const int minAge = 18;
            if (user.Age < minAge && newTransaction.Operation == (OperationType)OperationTypeDto.Income)
            {
                return Results.BadRequest(new Message ("User is under 18 years old."));
            }
            
            await db.Transactions.AddAsync(newTransaction);
            await db.SaveChangesAsync();
            
            var result = new TransactionResponseDto(newTransaction.Id, newTransaction.Description, newTransaction.Value, newTransaction.Operation, newTransaction.UserId);
            
            return Results.Created($"transactions/{newTransaction.Id}", result);
        })
            .Produces<TransactionResponseDto>(StatusCodes.Status201Created)
            .WithSummary("Create Transaction");

        transactionsEndpoint.MapDelete("{id:int}", 
                async (int id, AppDbContext db) =>
        {
            var transaction = await db.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return Results.NotFound(new Message ("Transaction not found."));
            }
            
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
            
            return Results.Ok(new Message ("Transaction successfully deleted."));
        })
            .Produces<Message>(statusCode: StatusCodes.Status200OK)
            .WithSummary("Delete Transaction");
    }
}
