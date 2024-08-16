using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LinkShortenerAPI.Hubs;

using Microsoft.AspNetCore.SignalR;

public class MainHub : Hub
{
    public static Dictionary<int, List<String>> ConnectedUsers = new Dictionary<int, List<String>>();

    public override Task OnConnectedAsync()
    {
        Console.WriteLine("onconnected HUB");
        var context = Context.GetHttpContext();
        if (context != null)
        {
            var token = context.Request.Query["access_token"];
            Console.WriteLine("token: " + token.ToString());
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var userId = int.Parse(jsonToken.Claims
                .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub || claim.Type == "nameid")?.Value);
            Console.WriteLine("userId: " + userId);
            var connectionId = Context.ConnectionId;

            if (!ConnectedUsers.ContainsKey(userId))
            {
                ConnectedUsers.Add(userId, new List<string>());
            }

            ConnectedUsers[userId].Add(connectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("ondisconnected HUB");
        var context = Context.GetHttpContext();
        if (context != null)
        {
            var token = context.Request.Query["access_token"];
            Console.WriteLine("token: " + token.ToString());
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var userId = int.Parse(jsonToken.Claims
                .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub || claim.Type == "nameid")?.Value);
            Console.WriteLine("userId: " + userId);
            var connectionId = Context.ConnectionId;
            if (ConnectedUsers.ContainsKey(userId))
            {
                ConnectedUsers[userId].Remove(connectionId);
                if (ConnectedUsers[userId].Count == 0)
                {
                    ConnectedUsers.Remove(userId);
                }
            }
        }

        return base.OnDisconnectedAsync(exception);
    }
}