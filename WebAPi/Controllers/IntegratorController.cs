using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Commands;
using SharedLibrary.Dto;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPi.Services;

namespace WebAPi.Controllers;

public class IntegratorController : ApiControllerBase
{
    public IConfiguration _config { get; }
    public IMemoryCache _cache { get; }
    private IHubContext<MessagingHub> _hubContext { get; }
    private readonly HttpClient _http;
    public string _machineToken { get; set; }
    public IntegratorController(IHubContext<MessagingHub> hubContext, IConfiguration config, HttpClient http, IMemoryCache cache)
    {
        _hubContext = hubContext;
        _config = config;
        _http = http;
        _cache = cache;
        _http.BaseAddress = new Uri(_config["Server"]);
        Generate();
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _machineToken);
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


    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] IntegratorModel model)
    {
        var loginResponse = _http.PostAsJsonAsync<IntegratorModel>($"api/Integrator/Login", model);
        string loginToken = await loginResponse.Result.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(loginToken))
        {
            return Ok(loginToken);
        }
        else
        {
            return NotFound("Not Authorized");
        }
    }

    [Authorize(Policy = "INTEGRATOR")]
    [HttpPost]
    public async Task<ActionResult<string>> SendOrder(OrderCommand command)
    {
        // send order
        var hash = User.Claims.FirstOrDefault(e => e.Type == "Hash")?.Value;
        var hasIntegratorClaim = Guid.TryParse(User.Claims.FirstOrDefault(e => e.Type == "INTEGRATOR")?.Value, out Guid integratorId);

        if (hasIntegratorClaim)
        {
            IntegratorModel model = new IntegratorModel()
            {
                Hash = hash,
            };

            var connectionIdResponse = _http.PostAsJsonAsync<IntegratorModel>($"api/Integrator/CheckIntegratorAvaiability", model);
            bool isIntegrator = await connectionIdResponse.Result.Content.ReadFromJsonAsync<bool>();
            if (isIntegrator)
            {
                var machinesLoggedIn = await _cache.GetOrCreateAsync("machinesLoggedIn", async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(30);
                    var listMachinesLoggedIn = new List<MachineRegistrationCommand>();

                    return listMachinesLoggedIn;
                });

                var machineLoggedInObj = machinesLoggedIn.FirstOrDefault(e => e.BrandId == command.BrandId && e.ZoneIds.Contains(command.ZoneId));

                if (machineLoggedInObj != null)
                {
                    bool isLoged = await OnSendToMachineBeingOffline(command, machineLoggedInObj, receivedAt: DateTime.Now);

                    if (isLoged)
                    {
                        await _hubContext.Clients.Client(machineLoggedInObj.ConnectionId).SendAsync("NewOrder", command.Notes);
                        return Ok("Order has been sent to Site");
                    }
                    return Ok("Error happened unexpectedly");
                }
                else
                {
                    bool isLoged = await OnSendToMachineBeingOffline(command, machineLoggedInObj);

                    if (isLoged)
                    {
                        return Ok("Site is offline now, but your order will proceed automatically when target site being online");
                    }
                    return Ok("Brand has no site on this zone");
                }
            }
            else
            {
                return Ok("Your are not registered as integrator");
            }
        }
        else
        {
            return Ok("Not authourized");
        }
    }

    private async Task<bool> OnSendToMachineBeingOffline(OrderCommand order, MachineRegistrationCommand machine, DateTime? receivedAt = null)
    {
        MachineMessageLogCommand request = new MachineMessageLogCommand()
        {
            SentAt = DateTime.Now,
            ReceivedAt = receivedAt,
            Payload = order.Notes,
            BrandId = order.BrandId,
            ZoneId = order.ZoneId,
            MachineName = machine?.MachineName,
            ConnectionId = machine?.ConnectionId,
            SiteId = machine?.SiteId,
            SiteHash = machine?.Hash,
        };
        var response = _http.PostAsJsonAsync($"api/Machine/OnSendMessagesToMachine", request);
        bool isLoged = await response.Result.Content.ReadFromJsonAsync<bool>();

        return isLoged;
    }

}