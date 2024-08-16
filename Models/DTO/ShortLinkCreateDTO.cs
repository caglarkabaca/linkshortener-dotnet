namespace LinkShortenerAPI.Models.DTO;

public class ShortLinkCreateDTO
{
    public string Name { get; set; }
    public required string RedirectUrl { get; set; }
    public string? UniqueCode { get; set; }
    
}