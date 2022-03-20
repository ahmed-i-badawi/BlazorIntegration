using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;

namespace WebAPi.Services;

public class MessagingHub : Hub
{
    public const string HubUrl = "/MessagingHub";

    public override Task OnConnectedAsync()
    {
        Console.WriteLine("cont: "+ Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception e)
    {
        await base.OnDisconnectedAsync(e);
    }

}