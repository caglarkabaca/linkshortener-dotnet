using System.ComponentModel;
using System.Text.Json;
using LinkShortenerAPI.Hubs;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LinkShortenerAPI.Controllers;

[ApiController]
// [Route("[controller]")]
public class RedirectController : Controller
{
    private ApiDbContext _context;
    private readonly IHubContext<MainHub> _hub;
    private readonly CouponService _couponService;

    public RedirectController(ApiDbContext context, IHubContext<MainHub> hub, CouponService couponService)
    {
        _context = context;
        _hub = hub;
        _couponService = couponService;
    }

    // GET
    [AllowAnonymous]
    [HttpGet("{uniqueCode}")]
    public async Task<IActionResult> RedirectLink([FromRoute] string uniqueCode)
    {
        var links = _context.ShortLinks.Where(l => l.UniqueCode.Equals(uniqueCode) && l.IsActive);
        if (!links.Any())
            return StatusCode(404, "Böyle bir link bulunamadı");

        var link = links.First();

        link.ClickCount += 1;

        var redirectTime = DateTime.UtcNow;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var headersDict = HttpContext.Request.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value));
        var headersJson = JsonSerializer.Serialize(headersDict);

        var shortLinkLogModel = new ShortLinkLog
        {
            ShortLink = link,
            IpAddress = ipAddress,
            RedirectTime = redirectTime,
            UserAgent = userAgent,
            HeadersJson = headersJson
        };

        await _context.ShortLinkLogs.AddAsync(
            shortLinkLogModel
        );

        await _context.SaveChangesAsync();

        Console.WriteLine("Redirect işlemi yapıldı. " + link.UniqueCode + " " + link.CreatedById);

        if (MainHub.ConnectedUsers.ContainsKey(link.CreatedById!.Value))
        {
            await _hub.Clients.Clients(MainHub.ConnectedUsers[link.CreatedById!.Value])
                .SendAsync("ReceieveLog#" + link.Id, JsonSerializer.Serialize(shortLinkLogModel));
            Console.WriteLine("Cliente yollandı. " + link.Id);
        }

        // --
        if ((await _couponService.GetCouponServices(link)).Count > 0)
            return Redirect("http://localhost:5173/" + link.UniqueCode);

        return View("~/Pages/Redirect/RedirectLink.cshtml", link);
    }
}