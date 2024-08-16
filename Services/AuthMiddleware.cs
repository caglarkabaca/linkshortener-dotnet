using LinkShortenerAPI.Controllers;
using LinkShortenerAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace LinkShortenerAPI.Services;

public class AuthMiddleware
{
    public readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApiDbContext dbContext, IUserService userService)
    {
        if (!context.Request.Headers["Authorization"].Any() || context.Request.Path.Value.StartsWith("/Auth"))
            await _next(context);
        else
        {
            var userName = context.User.Identity.Name;
            // var userQuery = dbContext.Users.Where(u => u.UserName.Equals(userName));
            // var userModel = await userQuery.FirstAsync();
            var userModel = (await userService.GetUsers()).FirstOrDefault(u => u.UserName.Equals(userName));
            if (userModel?.IsActive == true)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 403;
            }
        }
    }
}