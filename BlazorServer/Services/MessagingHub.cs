using BlazorServer.Data;
using BlazorServer.Data.Entities;
using BlazorServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Shared.Commands;
using Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
    public IConfiguration _config { get; }

    public MessagingHub(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public const string HubUrl = "/MessagingHub";
    private readonly IApplicationDbContext context;

    //private async Task<string> GenerateToken(string fingerPrint)
    //{
    //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
    //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //    var claims = new[]
    //    {
    //        new Claim(ClaimTypes.SerialNumber, fingerPrint),
    //        new Claim(ClaimTypes.Role, "Machine")
    //    };

    //    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
    //      _config["Jwt:Audience"],
    //      claims,
    //      expires: DateTime.Now.AddMinutes(999),
    //      signingCredentials: credentials);

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}

    private async Task<string> RegisterIfNotThenLogin(Machine machineobj, string newConnectionId)
    {
        // if not registered yet
        if (machineobj.CurrentStatus == MachineStatus.Pending)
        {
            // register current machine then login and stuaus to alive
            // need to wait server for registeration
            machineobj.ConnectionId = newConnectionId;
            _context.SaveChanges();
            return String.Empty;
        }
        // if registered
        else if (machineobj.CurrentStatus == MachineStatus.Closed || machineobj.CurrentStatus == MachineStatus.Approved)
        {
            // login current machine and stuaus to alive
           // var token = await GenerateToken(machineobj.FingerPrint);

            // token is valid
            if (!string.IsNullOrWhiteSpace(machineobj.FingerPrint))
            {
                //machineobj.CurrentStatus = MachineStatus.Alive;

                MachineLog log = new MachineLog()
                {
                    MachineId = machineobj.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Alive
                };
                _context.MachineLogs.Add(log);
                machineobj.ConnectionId = newConnectionId;
                machineobj.CurrentStatus = MachineStatus.Alive;
                _context.SaveChanges();
                return machineobj.FingerPrint;
            }
            // if token not valid
            else
            {
                return String.Empty;
            }
        }

        return "";
    }
    public override async Task<Task> OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var sysInfo = httpContext.Request.Query["sysInfo"].ToString();
        var connectionId = httpContext.Request.Query["id"].ToString();

        SystemInfo systemGuid = new SystemInfo();

        string machineFingerPrint = sysInfo.EncryptString();
        var machineobj = _context.Machines.FirstOrDefault(e => e.FingerPrint == machineFingerPrint);

        if (machineobj != null)
        {
            var token = await RegisterIfNotThenLogin(machineobj, connectionId);
        }
        else
        {
            try
            {
                Machine machine = new Machine()
                {
                    FingerPrint = machineFingerPrint,
                    //CurrentStatus = MachineStatus.Pending,
                    ConnectionId = connectionId,
                    MachineLogs = new List<MachineLog>()
                {
                    new MachineLog()
                    {
                        OccurredAt = DateTime.Now,
                        Status=MachineStatus.Pending,
                    }
                }
                };

                _context.Machines.Add(machine);
                _context.SaveChanges();

                var newMachineobj = _context.Machines.First(e => e.FingerPrint == machineFingerPrint);

                var token = await RegisterIfNotThenLogin(newMachineobj, connectionId);

            }
            catch (Exception)
            {

                throw;
            }

        }

        return base.OnConnectedAsync();
    }


    public override async Task OnDisconnectedAsync(Exception e)
    {
        var httpContext = Context.GetHttpContext();
        var sysInfo = httpContext.Request.Query["sysInfo"].ToString();
        var connectionId = httpContext.Request.Query["id"].ToString();
        SystemInfo systemGuid = new SystemInfo();
        string machineFingerPrint = sysInfo.EncryptString();

        var machineDisconnected = _context.Machines.FirstOrDefault(m => m.ConnectionId == connectionId && m.FingerPrint == machineFingerPrint);
        if (machineDisconnected != null)
        {
            // if registered
            if (machineDisconnected.CurrentStatus != MachineStatus.Pending)
            {
                MachineLog machineLog = new MachineLog()
                {
                    MachineId = machineDisconnected.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Closed
                };
                _context.MachineLogs.Add(machineLog);
                machineDisconnected.CurrentStatus = MachineStatus.Closed;
            }
            machineDisconnected.ConnectionId = string.Empty;
            _context.SaveChanges();
        }

        await base.OnDisconnectedAsync(e);
    }

}