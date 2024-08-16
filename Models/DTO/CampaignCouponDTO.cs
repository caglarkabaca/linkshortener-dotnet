namespace LinkShortenerAPI.Models.DTO;

public class CampaignCouponDTO
{
    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string RedirectUrl { get; set; }
    public string? CustomRedirectUrl { get; set; }
}