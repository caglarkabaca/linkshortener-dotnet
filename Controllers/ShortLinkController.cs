using System.Security.Claims;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using LinkShortenerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace LinkShortenerAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "user,admin")]
public class ShortLinkController : Controller
{
    private readonly ApiDbContext _context;
    private readonly IUserService _userService;

    public ShortLinkController(ApiDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    // GET '/'
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllLinks([FromQuery] string? nameSearch = null, [FromQuery] int page = 0,
        [FromQuery] int take = 50, [FromQuery] string? sortBy = null, [FromQuery] bool isDescending = true)
    {
        var query = _context.ShortLinks.AsQueryable();
        if (nameSearch != null)
            query = query.Where(l => EF.Functions.ILike(l.Name, $"%{nameSearch}%"));
        var totalCount = await query.CountAsync();

        if (isDescending)
            query = sortBy switch
            {
                "name" => query.OrderByDescending(l => l.Name),
                "click" => query.OrderByDescending(l => l.ClickCount),
                _ => query.OrderByDescending(l => l.CreateDate)
            };
        else
            query = sortBy switch
            {
                "name" => query.OrderBy(l => l.Name),
                "click" => query.OrderBy(l => l.ClickCount),
                _ => query.OrderBy(l => l.CreateDate)
            };


        var links = await query
            .Skip(page * take)
            .Take(take)
            .ToListAsync();

        return Ok(new ShortLinkListDTO
        {
            ShortLinks = links,
            Page = page,
            Take = take,
            TotalCount = totalCount
        });
    }

    // GET '/{id}'
    [Authorize(Roles = "admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var linkModel = await _context.ShortLinks.FirstOrDefaultAsync(e => e.Id == id);

        // linkModel null check
        if (linkModel != null)
            return Ok(linkModel);

        return BadRequest(new ErrorResponseDTO
        {
            ErrorMessage = "Girilen Id değeriyle eş bir ShortLink bulunamadı."
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById([FromRoute] int id)
    {
        if ((bool)User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value.Equals("user"))
        {
            var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var link = await _context.ShortLinks.FirstOrDefaultAsync(l => l.Id == id);
            if (link.CreatedById != userId)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    ErrorMessage = "Silmeye çalıştığınız kısa link, size ait değil"
                });
            }
        }

        var query = _context.ShortLinks.Where(l => l.Id == id);
        if (!await query.AnyAsync())
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Bu id'e ait bir link bulunamadı."
            });
        }

        var linkModel = await query.FirstOrDefaultAsync();
        linkModel!.IsActive = false;
        await _context.SaveChangesAsync();
        return Ok();
    }


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllLinksByUser([FromQuery] string? nameSearch = null, [FromQuery] int page = 0,
        [FromQuery] int take = 50, [FromQuery] string? sortBy = null, [FromQuery] bool isDescending = true)
    {
        var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var query = _context.ShortLinks.AsNoTracking().Where(l => l.CreatedById == userId && l.IsActive == true);
        if (nameSearch != null)
            query = query.Where(l => EF.Functions.ILike(l.Name, $"%{nameSearch}%"));
        var totalCount = await query.CountAsync();

        if (isDescending)
            query = sortBy switch
            {
                "name" => query.OrderByDescending(l => l.Name),
                "click" => query.OrderByDescending(l => l.ClickCount),
                _ => query.OrderByDescending(l => l.CreateDate)
            };
        else
            query = sortBy switch
            {
                "name" => query.OrderBy(l => l.Name),
                "click" => query.OrderBy(l => l.ClickCount),
                _ => query.OrderBy(l => l.CreateDate)
            };

        var links = await query
            .Skip(page * take)
            .Take(take)
            .ToListAsync();

        return Ok(new ShortLinkListDTO
        {
            ShortLinks = links,
            Page = page,
            Take = take,
            TotalCount = totalCount
        });
    }

    // POST '/'
    [HttpPost]
    public async Task<IActionResult> CreateLink([FromBody] ShortLinkCreateDTO shortLinkDto)
    {
        // guid oluşturup gelen shortlinkin üstüne yazıyor
        shortLinkDto.UniqueCode ??= Guid.NewGuid().ToString();
        try
        {
            var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            // dbye ekleme işlemleri
            var modelEntry = await _context.ShortLinks.AddAsync(new ShortLink
            {
                Name = shortLinkDto.Name,
                RedirectUrl = shortLinkDto.RedirectUrl,
                UniqueCode = shortLinkDto.UniqueCode,
                CreatedById = userId,
            });
            await _context.SaveChangesAsync();

            // oluşturulan uniq kodu döndürür
            return Ok(modelEntry.Entity);
        }
        catch (Exception e)
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Eşsiz kod oluşturma hatası aldınız, lütfen daha sonra tekrar deneyiniz."
            }.ToString());
        }
    }
}