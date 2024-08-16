namespace LinkShortenerAPI.Models.DTO;

public class ShortLinkListDTO
{
    public List<ShortLink> ShortLinks { get; set; }
    public int Page { get; set; }
    public int Take { get; set; }
    public int TotalCount { get; set; }
}