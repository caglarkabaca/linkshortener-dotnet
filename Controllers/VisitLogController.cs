using System.Text.Json;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortenerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class VisitLogController : Controller
{
    private readonly ApiDbContext _context;

    public VisitLogController(ApiDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task<IActionResult> Index([FromBody] VisitLogCreateDTO dto)
    {
        try
        {
            var logTime = DateTime.UtcNow;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            
            var headersDict = HttpContext.Request.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value));
            var headersJson = JsonSerializer.Serialize(headersDict);
            await _context.VisitLogs.AddAsync(new VisitLog()
            {
                From = dto.From,
                To = dto.To,
                HtmlTag = dto.HtmlTag,
                HtmlTagId = dto.HtmlTagId,
                HtmlTagName = dto.HtmlTagName,
                HtmlTagRaw = dto.HtmlTagRaw,
                
                UserAgent = headersJson,
                IpAddress = ipAddress,
                LogTime = logTime
            });
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
        
        return Ok();
    }
}