using System.ComponentModel.DataAnnotations;

namespace MinimalApi.DTOs;
public class UserDto(int id, string name, int age)
{
    [Required] 
    public int Id { get; set; } = id;

    [Required]
    public string Name { get; set; } = name;

    [Required]
    public int Age { get; set; } = age;
}
public class UserRequestDto 
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public int Age { get; set; }
    
}
public record UserListResponseDto(List<UserDto> Users);
public record UserListResponseWithTransactionsDto(List<UserDto> Users, List<TransactionDto> Transactions);
public record Message (string message);