using BlazorServer.Data;
using BlazorServer.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;
using Shared.Enums;

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
            MachineLog machineLog = new MachineLog()
            {
                MachineId = machineDisconnected.Id,
                OccurredAt = DateTime.Now,
                Status = MachineStatus.Closed
            };
            _context.MachineLogs.Add(machineLog);
            _context.SaveChanges();
        }

        await base.OnDisconnectedAsync(e);
    }

}