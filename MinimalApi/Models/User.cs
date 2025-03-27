using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Models;

public class User
{
    public int Id { get; init; }
    
    public  string Name { get; set; }
    public int Age { get; set; }
    public  List<Transaction> Transactions {get; set;}

    public User(string name, int age)
    {
        Name = name;
        Age = age;
    }
}