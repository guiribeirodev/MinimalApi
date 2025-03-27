using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MinimalApi.Models;

namespace MinimalApi.DTOs;

public class TransactionDto(int id, string description,decimal value, OperationType operation, int userId)
{
    [JsonPropertyName("id")] 
    public int Id { get; set; } = id;

    [JsonPropertyName("description")]
    [Required]
    public string Description { get; set; } = description;

    [JsonPropertyName("value")] [Required] 
    public decimal Value { get; set; } = value;

    [JsonPropertyName("operation")]
    [Required]
    public OperationType Operation { get; set; } = operation;

    [JsonPropertyName("user_id")] public int UserId { get; set; } = userId;
}

public class TransactionRequestDto
{
    [JsonPropertyName("description")]
    [Required]
    public required string Description { get; set; }

    [JsonPropertyName("value")]
    [Required]
    public decimal Value { get; set; }

    [JsonPropertyName("operation")]
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OperationType Operation { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
}
public record TransactionResponseDto(int Id, string Description, decimal Value, OperationType Operation, int UserId);
public record TransactionListResponseDto(List<TransactionResponseDto> Transactions);