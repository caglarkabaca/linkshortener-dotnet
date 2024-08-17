namespace LinkShortenerAPI.Models;

public class UserCampaignSetting
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public string LogoUrl { get; set; }
    public string BackgroundColor { get; set; }
    public string StartButtonText { get; set; }
    public string EndButtonText { get; set; }
    
    public string SuccessText { get; set; }
}