namespace LinkShortenerAPI.Models.DTO;

public class CampaignCouponResponseDTO
{
    public List<CampaignCouponDTO> Coupons { get; set; }
    public UserCampaignSettingDTO? Setting { get; set; }
}