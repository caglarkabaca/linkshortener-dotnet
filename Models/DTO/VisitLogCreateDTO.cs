namespace LinkShortenerAPI.Models.DTO;

public class VisitLogCreateDTO
{
    public string? From { get; set; }
    public string? To { get; set; }
    
    public string? HtmlTagId { get; set; }
    public string? HtmlTagName { get; set; }
    public string? HtmlTag { get; set; }
    public string? HtmlTagRaw { get; set; }
}