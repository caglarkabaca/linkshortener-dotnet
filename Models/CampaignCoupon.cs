namespace LinkShortenerAPI.Models;

public class CampaignCoupon
{
    public int Id { get; set; }
    public int ShortLinkId { get; set; }
    public ShortLink ShortLink { get; set; }

    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string? CustomRedirectUrl { get; set; }

//     "title": `%5${i} İndirim`,
//     "code": `KUPON5${i}`,
//      "description": `Bu kodu kullanarak 1500TL üzeri her sepette geçerli %5${i} indirim kazandınız`,
//     custom redirect url
}