using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LinkShortenerAPI.Models;

public class ApiDbContext : DbContext
{
    public DbSet<ShortLink> ShortLinks { get; set; }
    public DbSet<ShortLinkLog> ShortLinkLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CampaignCoupon> CampaignCoupons { get; set; }
    public DbSet<UserCampaignSetting> UserCampaignSettings { get; set; }
    public DbSet<VisitLog> VisitLogs { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    //     => options.UseNpgsql(new NpgsqlConnection());
}