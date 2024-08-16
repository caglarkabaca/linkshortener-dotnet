namespace LinkShortenerAPI.Models.DTO;

public class ShortLinkLogListDTO
{
    public List<ShortLinkLog> ShortLinkLogs { get; set; }
    public int Page { get; set; }
    public int Take { get; set; }
    public int TotalCount { get; set; }
}