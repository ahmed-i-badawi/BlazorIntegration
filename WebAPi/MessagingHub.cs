using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Shared.Commands;
using Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

namespace WebAPi.Services;

public static class UserHandler
{
    public static List<MessageCommand> Connections = new()
    {
    };
    public static int InitStatusId = 1;
}


public class MessagingHub : Hub
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public IConfiguration _config { get; }
    public string HubUrl = "";
    public string _machineToken { get; set; }
    public HashSet<MachineRegistrationCommand>? PendingRegisterationBrands { get; set; }

    public MessagingHub(IConfiguration config, IHttpClientFactory http, IMemoryCache cache)
    {
        _config = config;
        _cache = cache;
        _http = http.CreateClient();
        _http.BaseAddress = new Uri(_config["Server"]);
        HubUrl = _config["HubUrl"];
        Generate();
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _machineToken);
        PendingRegisterationBrands = new HashSet<MachineRegistrationCommand>();
    }

    private void Generate()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("MachineToMachine", "MachineToMachine"),
            };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Audience"],
          claims,
          expires: DateTime.Now.AddMinutes(999),
          signingCredentials: credentials);
        _machineToken = new JwtSecurityTokenHandler().WriteToken(token);
    }


    public override async Task<Task> OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var sysInfo = httpContext.Request.Query["sysInfo"].ToString();
        var connectionId = Context.ConnectionId;


        var isChache = _cache.TryGetValue("pendingMachineRegistration", out List<MachineRegistrationCommand> pendingMachinesRegistration);
        if (isChache)
        {
            var machine = pendingMachinesRegistration.FirstOrDefault(e => e.SystemInfo == sysInfo);
            if (machine != null)
            {
                object myObj = new
                {
                    sysInfo = sysInfo,
                    connectionId = connectionId,
                    hash = machine.Hash,
                    machineName = machine.MachineName,
                    notes = machine.Notes,
                };

                var machineObjResponse = _http.PostAsJsonAsync<object>($"api/Machine/OnMachineConnect", myObj);
                bool machineObjRes = await machineObjResponse.Result.Content.ReadFromJsonAsync<bool>();
                if (machineObjRes)
                {
                    _cache.Remove("pendingMachineRegistration");
                    pendingMachinesRegistration.Remove(machine);
                    _cache.Set("pendingMachineRegistration", pendingMachinesRegistration, DateTime.UtcNow.AddDays(1));

                    await this.Clients.Client(connectionId).SendAsync("MachineIsAdded", $"machine {machine.MachineName}: added successfully");
                }
            }
        }

        return base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception e)
    {
        var httpContext = Context.GetHttpContext();
        var sysInfo = httpContext?.Request?.Query?["sysInfo"].ToString();
        var connectionId = Context.ConnectionId;

        object myObj = new
        {
            sysInfo = sysInfo,
            connectionId = connectionId,
        };

        var machineObjResponse = _http.PostAsJsonAsync<object>($"api/Machine/OnMachineDisConnect", myObj);
        bool machineObjRes = await machineObjResponse.Result.Content.ReadFromJsonAsync<bool>();

        await base.OnDisconnectedAsync(e);
    }

}