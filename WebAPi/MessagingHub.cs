using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SharedLibrary.Commands;
using SharedLibrary.Dto;
using SharedLibrary.Enums;
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
        var siteHash = httpContext.Request.Query["siteHash"].ToString();
        var connectionId = Context.ConnectionId;

        //var isChache = _cache.TryGetValue("pendingMachineRegistration", out List<MachineRegistrationCommand> pendingMachinesRegistration);


        //var machine = pendingMachinesRegistration?.FirstOrDefault(e => e.SystemInfo == sysInfo);

        MachineModel myObj = new MachineModel()
        {
            SystemInfo = sysInfo,
            ConnectionId = connectionId,
            Hash = siteHash
        };

        var machineObjResponse = _http.PostAsJsonAsync<MachineModel>($"api/Machine/OnMachineConnect", myObj);
        MachineDto machineObjRes = await machineObjResponse.Result.Content.ReadFromJsonAsync<MachineDto>();

        if (machineObjRes?.SiteId == null)
        {
            await this.Clients.Client(connectionId).SendAsync("MachineIsAdded", $"Trying Again");
        }

        if (machineObjRes?.SiteId.GetValueOrDefault() > 0)
        {
            //_cache.Remove("pendingMachineRegistration");
            //pendingMachinesRegistration?.Remove(machine);
            //_cache.Set("pendingMachineRegistration", pendingMachinesRegistration, DateTime.UtcNow.AddDays(30));

            //await this.Clients.Client(connectionId).SendAsync("MachineIsAdded", $"machine {machine.MachineName}: added successfully and logged in");
        }
        if (machineObjRes?.SiteId > 0)
        {
            var machinesLoggedIn = await _cache.GetOrCreateAsync("machinesLoggedIn", async entry =>
            {
                entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(30);
                var listMachinesLoggedIn = new List<MachineRegistrationCommand>();

                return listMachinesLoggedIn;
            });

            if (!machinesLoggedIn.Any(e => e.ConnectionId == connectionId && e.SystemInfo == myObj.SystemInfo))
            {
                machinesLoggedIn.Add(new MachineRegistrationCommand()
                {
                    SystemInfo = myObj.SystemInfo,
                    ConnectionId = connectionId,
                    BrandId = machineObjRes.BrandId,
                    SiteId = machineObjRes.SiteId,
                    ZoneIds = machineObjRes.ZoneIds,
                });

                _cache.Remove("machinesLoggedIn");

                _cache.Set("machinesLoggedIn", machinesLoggedIn, DateTime.UtcNow.AddDays(30));
            }

            await this.Clients.Client(connectionId).SendAsync("MachineIsLoggedIn", true);

            try
            {
                var ordersObj = _http.GetFromJsonAsync<List<string>>($"api/Machine/GetMachinePendingOrdersWhenBeingOnline?brandId={machineObjRes.BrandId}&siteId={machineObjRes.SiteId}");
                var orders = await ordersObj;

                if (orders?.Any() ?? false)
                {
                    foreach (var item in orders)
                    {
                        await this.Clients.Client(connectionId).SendAsync("NewOrder", item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        return base.OnConnectedAsync();
    }

    //private async Task<bool> OnSendToMachineBeingOffline(OrderCommand order, MachineRegistrationCommand machine, DateTime? receivedAt = null)
    //{
    //    MachineMessageLogCommand request = new MachineMessageLogCommand()
    //    {
    //        SentAt = DateTime.Now,
    //        ReceivedAt = receivedAt,
    //        Payload = order.Notes,
    //        BrandId = order.BrandId,
    //        ZoneId = order.ZoneId,
    //        MachineName = machine.MachineName,
    //        ConnectionId = machine.ConnectionId,
    //        SiteId = machine.SiteId,
    //        SiteHash = machine.Hash,
    //    };
    //    var response = _http.PostAsJsonAsync($"api/Machine/OnSendToMachineBeingOffline", request);
    //    bool isLoged = await response.Result.Content.ReadFromJsonAsync<bool>();

    //    return isLoged;
    //}

    public override async Task OnDisconnectedAsync(Exception e)
    {
        var httpContext = Context.GetHttpContext();
        var sysInfo = httpContext?.Request?.Query?["sysInfo"].ToString();
        var connectionId = Context.ConnectionId;

        MachineModel myObj = new MachineModel()
        {
            SystemInfo = sysInfo,
            ConnectionId = connectionId,
        };

        var machineObjResponse = _http.PostAsJsonAsync<object>($"api/Machine/OnMachineDisConnect", myObj);
        bool machineObjRes = await machineObjResponse.Result.Content.ReadFromJsonAsync<bool>();

        var machinesLoggedIn = await _cache.GetOrCreateAsync("machinesLoggedIn", async entry =>
        {
            entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(30);
            var listMachinesLoggedIn = new List<MachineRegistrationCommand>();

            return listMachinesLoggedIn;
        });

        var machineLoggedInObj = machinesLoggedIn.FirstOrDefault(e => e.ConnectionId == connectionId && e.SystemInfo == myObj.SystemInfo);
        if (machineLoggedInObj != null)
        {
            machinesLoggedIn.Remove(machineLoggedInObj);

            _cache.Remove("machinesLoggedIn");

            _cache.Set("machinesLoggedIn", machinesLoggedIn, DateTime.UtcNow.AddDays(30));
        }

        await base.OnDisconnectedAsync(e);
    }

}