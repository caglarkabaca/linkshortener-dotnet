using LinkShortenerAPI.Models.DTO;
using LinkShortenerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortenerAPI.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class CampaignCouponController : Controller
{
    private readonly CouponService _service;

    public CampaignCouponController(CouponService service)
    {
        _service = service;
    }

    [HttpGet("{uniqueCode}")]
    public async Task<IActionResult> Index([FromRoute] string uniqueCode)
    {
        return Ok(await _service.GetCouponServices(uniqueCode));
    }
}