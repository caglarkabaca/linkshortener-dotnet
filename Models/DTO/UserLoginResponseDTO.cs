namespace LinkShortenerAPI.Models.DTO;

public class UserLoginResponseDTO
{
    public UserResponseDTO User { get; set; }
    public string? Token { get; set; }
}