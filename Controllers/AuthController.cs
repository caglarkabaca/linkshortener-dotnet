using System.ComponentModel;
using LinkShortenerAPI.Models.DTO;
using LinkShortenerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortenerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "user,admin")]
    [HttpGet("Validate")]
    public IActionResult Validate()
    {
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDto)
    {
        var userLoginResponseDto = await _userService.Login(userLoginDto.UserName, userLoginDto.Password);

        if (userLoginResponseDto == null || userLoginResponseDto.Token == null)
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Kullanıcı adı veya şifre hatalı, lütfen tekrar deneyiniz."
            });
        }

        if (!userLoginResponseDto.User.IsActive)
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Giriş yapmaya çalıştığınız hesap yasaklı."
            });
        }

        return Ok(userLoginResponseDto);
    }

    [AllowAnonymous]
    [HttpPost("LoginPhone")]
    public async Task<IActionResult> LoginPhone([FromBody] UserLoginPhoneDTO userLoginDto)
    {
        var userLoginResponseDto = await _userService.Login(userLoginDto.PhoneNumber);

        if (userLoginResponseDto == null || userLoginResponseDto.Token == null)
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Bu telefon numarasına ait hesap bulunamadı, lütfen tekrar deneyiniz."
            });
        }

        if (!userLoginResponseDto.User.IsActive)
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Giriş yapmaya çalıştığınız hesap yasaklı."
            });
        }

        return Ok(userLoginResponseDto);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDTO userCreateDto)
    {
        var userLoginResponseDto = await _userService.Register(userCreateDto);

        if (userLoginResponseDto == null)
        {
            return BadRequest(new ErrorResponseDTO
            {
                ErrorMessage = "Kullanıcı mevcut, lütfen kendi hesabınızla giriş yapınız."
            });
        }

        return Ok(userLoginResponseDto);
    }
}