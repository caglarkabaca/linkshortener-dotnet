using System.Security.Claims;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkShortenerAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "admin, user")]
public class ShortLinkLogController : Controller
{
    private readonly ApiDbContext _context;

    public ShortLinkLogController(ApiDbContext context)
    {
        _context = context;
    }

    // GET '/'
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var links = await _context.ShortLinkLogs.OrderBy(l => l.Id).ToListAsync();
        return Ok(links);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLogsByShortLinkId([FromRoute] int id, [FromQuery] int page = 0,
        [FromQuery] int take = 50, [FromQuery] bool isDescending = true)
    {
        if ((bool)User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value.Equals("user"))
        {
            var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var link = await _context.ShortLinks.FirstOrDefaultAsync(l => l.Id == id);
            if (link.CreatedById != userId)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    ErrorMessage = "Kayıtlarına ulaşmaya çalıştığınız kısa link, size ait değil"
                });
            }
        }

        var query = _context.ShortLinkLogs.Where(log => log.ShortLinkId == id);
        var totalCount = await query.CountAsync();
        if (isDescending)
            query = query.OrderByDescending(l => l.RedirectTime);
        else
            query = query.OrderBy(l => l.RedirectTime);

        var linkLogs = await query
            .Skip(page * take)
            .Take(take)
            .ToListAsync();

        return Ok(new ShortLinkLogListDTO
        {
            ShortLinkLogs = linkLogs,
            Page = page,
            Take = take,
            TotalCount = totalCount
        });
    }
}