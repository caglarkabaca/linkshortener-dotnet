using Microsoft.EntityFrameworkCore;

namespace LinkShortenerAPI.Models;

[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(PhoneNumber), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; } = "user";
    public bool IsActive { get; set; } = true;
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}