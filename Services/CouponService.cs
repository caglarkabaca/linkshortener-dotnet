using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace LinkShortenerAPI.Services;

public class CouponService
{
    private readonly ApiDbContext _context;

    public CouponService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<List<CampaignCouponDTO>> GetCouponServices(ShortLink link)
    {
        var query = _context.CampaignCoupons.Where(c => c.ShortLinkId == link.Id);
        var list = await query.ToListAsync();
        return list.ConvertAll(c => new CampaignCouponDTO
        {
            Title = c.Title,
            Code = c.Code,
            Description = c.Description,
            RedirectUrl = link.RedirectUrl,
            CustomRedirectUrl = c.CustomRedirectUrl
        });
    }

    public async Task<List<CampaignCouponDTO>> GetCouponServices(String uniqueId)
    {
        var link = await _context.ShortLinks.Where(l => l.UniqueCode == uniqueId).FirstOrDefaultAsync();
        if (link == null)
            return new List<CampaignCouponDTO>(); // err handling
        return await GetCouponServices(link);
    }
    
    public async Task<UserCampaignSettingDTO?> GetUserSettings(String uniqueId)
    {
        var link = await _context.ShortLinks.Where(l => l.UniqueCode == uniqueId).FirstOrDefaultAsync();
        if (link == null)
            return null;
        var setting = await _context.UserCampaignSettings.Where(s => s.UserId == link.CreatedById).FirstOrDefaultAsync();
        if (setting == null)
            return null;
        return new UserCampaignSettingDTO
        {
            BackgroundColor = setting.BackgroundColor,
            EndButtonText = setting.EndButtonText,
            LogoUrl = setting.LogoUrl,
            SuccessText = setting.SuccessText,
            StartButtonText = setting.StartButtonText
        };
    }
}