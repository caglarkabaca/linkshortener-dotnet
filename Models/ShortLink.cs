namespace LinkShortenerAPI.Models;

//Id, yönlendirilecek adres, uniq code, create_date, update_date, status, click count
public class ShortLink
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string RedirectUrl { get; set; }
    public string UniqueCode { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime UpdateDate { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    public int ClickCount { get; set; }
}