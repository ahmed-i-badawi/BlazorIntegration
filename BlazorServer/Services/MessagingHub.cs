using BlazorServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;

namespace BlazorServer.Services;

public static class UserHandler
{
    public static List<MessageCommand> Connections = new()
    {
    };
    public static int InitStatusId = 1;
}


public class MessagingHub : Hub
{
    private readonly ApplicationDbContext _context;

    public MessagingHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public const string HubUrl = "/MessagingHub";
    private readonly IApplicationDbContext context;

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }


    public override async Task OnDisconnectedAsync(Exception e)
    {
        var machineDisconnected = _context.Machines.FirstOrDefault(e => e.ConnectionId == e.ConnectionId);
        if (machineDisconnected != null)
        {
            _context.Machines.Remove(machineDisconnected);
            _context.SaveChanges();
        }

        await base.OnDisconnectedAsync(e);
    }

}