namespace LinkShortenerAPI.Models;

public class ShortLinkLog
{
    public int Id { get; set; }
    public int ShortLinkId { get; set; }
    public ShortLink ShortLink { get; set; }
    public DateTime RedirectTime { get; set; }
    public string UserAgent { get; set; }
    public string IpAddress { get; set; }

    //
    public string HeadersJson { get; set; }
}