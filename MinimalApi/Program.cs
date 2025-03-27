using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:8000");
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("Minimal") ?? "Data Source=Minimal.db";
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Description = "MinimalApi";
    config.Title = "MinimalApi v1";
    config.Version = "v1";
});

var app = builder.Build();

app.UseCors("ReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "MinimalApi";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}


app.UseDefaultFiles();
if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
{
    app.UseStaticFiles();
}

app.UserEndpoints();
app.TransactionEndpoints();

app.Run();
public partial class Program { }
