using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace LinkShortenerAPI.Services;

public class UserService : IUserService
{
    private readonly ApiDbContext _context;
    private readonly JwtOptions _jwtOptions;

    private readonly IMemoryCache _memoryCache;
    public string cacheKey = "users";

    public UserService(ApiDbContext context, JwtOptions jwtOptions, IMemoryCache memoryCache)
    {
        _context = context;
        _jwtOptions = jwtOptions;
        _memoryCache = memoryCache;
    }

    public async Task<UserLoginResponseDTO?> Login(string userName, string password)
    {
        var user = _context.Users.Where(u => u.UserName.Equals(userName) && u.Password.Equals(password));
        if (!await user.AnyAsync())
            return null;

        var userModel = await user.FirstAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new(ClaimTypes.Name, userName),
                new(ClaimTypes.Email, userModel.Email),
                new(ClaimTypes.MobilePhone, userModel.PhoneNumber),
                new(ClaimTypes.Role, userModel.Role)
            }),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.Now.Add(TimeSpan.FromDays(1)),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        userModel.Token = tokenHandler.WriteToken(token);
        await _context.SaveChangesAsync();

        return new UserLoginResponseDTO
        {
            User = new UserResponseDTO
            {
                UserName = userModel.UserName,
                Email = userModel.PhoneNumber,
                PhoneNumber = userModel.PhoneNumber,
                IsActive = userModel.IsActive,
            },
            Token = userModel.Token
        };
    }

    public async Task<UserLoginResponseDTO?> Login(string phoneNumber)
    {
        var user = _context.Users.Where(u => u.PhoneNumber.Equals(phoneNumber));
        if (!await user.AnyAsync())
            return null;

        var userModel = await user.FirstAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new(ClaimTypes.Name, userModel.UserName),
                new(ClaimTypes.Email, userModel.Email),
                new(ClaimTypes.MobilePhone, userModel.PhoneNumber),
                new(ClaimTypes.Role, userModel.Role)
            }),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.Now.Add(TimeSpan.FromDays(1)),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        userModel.Token = tokenHandler.WriteToken(token);
        await _context.SaveChangesAsync();

        return new UserLoginResponseDTO
        {
            User = new UserResponseDTO
            {
                UserName = userModel.UserName,
                Email = userModel.PhoneNumber,
                PhoneNumber = userModel.PhoneNumber,
                IsActive = userModel.IsActive,
            },
            Token = userModel.Token
        };
    }

    public async Task<UserLoginResponseDTO?> Register(UserCreateDTO userCreateDto)
    {
        var user = await GetUserByUserName(userCreateDto.UserName);
        if (user != null)
            return null;

        try
        {
            var userEntity = await _context.Users.AddAsync(new User
            {
                UserName = userCreateDto.UserName,
                Password = userCreateDto.Password,
                Role = userCreateDto.Role ?? "user",
                Email = userCreateDto.Email,
                PhoneNumber = userCreateDto.PhoneNumber
            });
            await _context.SaveChangesAsync();

            return await Login(userEntity.Entity.UserName, userEntity.Entity.Password);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<List<User>> GetUsers()
    {
        List<User> users;
        if (!_memoryCache.TryGetValue(cacheKey, out users))
        {
            Console.WriteLine("Cache renewing...");
            users = await _context.Users.ToListAsync();
            _memoryCache.Set(cacheKey, users,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(15)));
        }
        else
        {
            Console.WriteLine("Cache used...");
        }

        return users;
    }

    public async Task<User?> GetUserByUserName(string userName)
    {
        var userQuery = _context.Users.Where(u => u.UserName.Equals(userName));
        if (await userQuery.AnyAsync()) return await userQuery.FirstAsync();

        return null;
    }
}