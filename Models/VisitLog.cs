namespace LinkShortenerAPI.Models;

public class VisitLog
{
    public int Id { get; set; }
    
    // Siteden alınan bilgiler
    public string? From { get; set; }
    public string? To { get; set; }
    public string? HtmlTagId { get; set; }
    public string? HtmlTagName { get; set; }
    public string? HtmlTag { get; set; }
    public string? HtmlTagRaw { get; set; }
    
    // Diğer bilgiler
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime LogTime { get; set; }
}