using Microsoft.EntityFrameworkCore;

namespace LinkShortenerAPI.Models.DTO;

public class UserResponseDTO
{
    public string UserName { get; set; }
    public bool IsActive { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}