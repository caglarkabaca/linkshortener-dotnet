using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using LinkShortenerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortenerAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "admin")]
public class UserController : Controller
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetUsers();
        return Ok(users);
    }
}