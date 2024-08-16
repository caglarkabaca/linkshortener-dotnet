namespace LinkShortenerAPI.Models.DTO;

public class UserCreateDTO
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string? Role { get; set; } = "user";
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}