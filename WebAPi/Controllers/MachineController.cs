using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Commands;
using Shared.Dto;
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
    private readonly HttpClient _http;
    public string _machineToken { get; set; }
    public MachineController(IConfiguration config, HttpClient http, IMemoryCache cache)
    {
        _config = config;
        _http = http;
        _cache = cache;
        Generate();
        _http.BaseAddress = new Uri(_config["Server"]);
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


    [HttpPost]
    public async Task<ActionResult<string>> RegisterMachine([FromBody] MachineRegistrationCommand command)
    {

        var pendingMachinesRegistration = await _cache.GetOrCreateAsync("pendingMachineRegistration", async entry =>
        {
            entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(30);
            var listMachineRegistration = new List<MachineRegistrationCommand>();

            return listMachineRegistration;
        });

        HashCheckerDto result = new HashCheckerDto();
        var response = _http.PostAsJsonAsync<string>($"api/Machine/HashChecker", command.Hash);
        try
        {
            result = await response.Result.Content.ReadFromJsonAsync<HashCheckerDto>();
        }
        catch (Exception ex)
        {

            throw;
        }


        if (!string.IsNullOrWhiteSpace(result.SystemInfo))
        {
            if (!pendingMachinesRegistration.Any(e => e.Hash == command.Hash && e.SystemInfo == result.SystemInfo))
            {
                pendingMachinesRegistration.Add(new MachineRegistrationCommand()
                {
                    Hash = command.Hash,
                    SystemInfo = result.SystemInfo,
                    MachineName = command.MachineName,
                    Notes = command.Notes
                });

                _cache.Remove("pendingMachineRegistration");

                _cache.Set("pendingMachineRegistration", pendingMachinesRegistration, DateTime.UtcNow.AddDays(30));

                return Ok("Start Your Worker Now");
            }
            else
            {
                return Ok("Hash is already Registered on this machine Before, Start Your Worker Now");
            }
        }
        else
        {
            return NotFound(result.Message);
        }
    }
}