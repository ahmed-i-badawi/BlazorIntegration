using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Commands;
using Shared.Enums;
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

public class MachineController : ApiControllerBase
{
    public IConfiguration _config { get; }
    public IMemoryCache _cache { get; }
    private IHubContext<MessagingHub> _hubContext { get; }
    private readonly HttpClient _http;
    public string _machineToken { get; set; }
    public MachineController(IHubContext<MessagingHub> hubContext, IConfiguration config, HttpClient http, IMemoryCache cache)
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
    public async Task<ActionResult> Login([FromBody] BrandModel brandLogin)
    {
        var loginResponse = _http.PostAsJsonAsync<BrandModel>($"api/Message/Login", brandLogin);
        var login = await loginResponse.Result.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(login))
        {
            return Ok(login);
        }
        else
        {
            return NotFound("No token");
        }
    }

    [Authorize(Policy = "Brand")]
    [HttpPost]
    public async Task<ActionResult<string>> GetMessageStatus()
    {
        // send order
        var hash = User.Claims.FirstOrDefault(e => e.Type == "Hash")?.Value;
        var brand = int.TryParse(User.Claims.FirstOrDefault(e => e.Type == "Brand")?.Value, out int brandInt);

        if (brand)
        {
            var connectionIdResponse = _http.PostAsJsonAsync<int>($"api/Message/GetMessageStatus", brandInt);
            var connectionId = await connectionIdResponse.Result.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("NewOrder", "");
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        return NotFound("brand Not Found");
    }

    [HttpPost]
    public async Task<ActionResult<string>> RegisterMachine([FromBody] MachineRegistrationCommand command)
    {
        var pendingMachinesRegistration = await _cache.GetOrCreateAsync("pendingMachineRegistration", async entry =>
        {
            entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(30);
            var listMachineRegistration = new List<MachineRegistrationCommand>();

            return listMachineRegistration;
        });


        var response = _http.PostAsJsonAsync<MachineRegistrationCommand>($"api/Machine/HashChecker", command);
        var systemInfo = await response.Result.Content.ReadAsStringAsync();

        // ----------
        if (!string.IsNullOrWhiteSpace(systemInfo))
        {
            if (!pendingMachinesRegistration.Any(e => e.Hash == command.Hash && e.SystemInfo == systemInfo))
            {
                pendingMachinesRegistration.Add(new MachineRegistrationCommand()
                {
                    Hash = command.Hash,
                    SystemInfo = systemInfo,
                    MachineName = command.MachineName,
                    Notes = command.Notes
                });

                _cache.Remove("pendingMachineRegistration");

                _cache.Set("pendingMachineRegistration", pendingMachinesRegistration, DateTime.UtcNow.AddDays(1));

                return Ok("Start Your Worker Now");
            }
            else
            {
                return Ok("Hash is already Registered on this machine Before, Start Your Worker Now");
            }
        }
        else
        {
            return NotFound("Hash Not Found");
        }
    }
}