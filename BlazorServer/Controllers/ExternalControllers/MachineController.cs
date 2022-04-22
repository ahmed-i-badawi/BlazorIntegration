using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Commands;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Dto;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using BlazorServer.Extensions;
using SharedLibrary.Entities;
using Infrastructure.LogDatabase.Common.Interfaces;
using BlazorServer.Services;

namespace BlazorServer.Controllers;

[Authorize]
public class MachineController : ApiControllerBase
{
    private readonly IApplicationDbContext _context;
    public IConfiguration _config { get; }
    public ILogDbContext _logDbContext { get; }

    public MachineController(IApplicationDbContext context,
        IConfiguration config,
        ILogDbContext logDbContext
        )
    {
        _context = context;
        _config = config;
        _logDbContext = logDbContext;
    }

    [HttpPost]
    public async Task<ActionResult<HashCheckerDto>> HashChecker([FromBody] string id)
    {
        HashCheckerDto res = new HashCheckerDto();

        if (Guid.TryParse(id, out Guid guid))
        {
            var brandObj = _context.Sites.Include(e => e.Machine).FirstOrDefault(e => e.Hash == guid);

            if (brandObj != null)
            {
                if (brandObj.Machine != null)
                {
                    res.SystemInfo = string.Empty;
                    res.Message = "this Site has a machine";

                    return Ok(res);
                }
                else
                {
                    SystemGuid systemGuid = new SystemGuid();
                    var systemInfo = systemGuid.SystemInfoAsync();

                    res.SystemInfo = systemInfo;
                    res.Message = string.Empty;

                    return Ok(res);
                }
            }
        }
        res.SystemInfo = string.Empty;
        res.Message = "No hash available";

        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<MachineDto>> OnMachineConnect([FromBody] MachineModel machineModel)
    {

        MachineDto result = new MachineDto();

        SystemInfo systemGuid = new SystemInfo();

        string machineFingerPrint = machineModel.SystemInfo.EncryptString();
        var machineobj = _context.Machines
            .Include(e => e.Site).ThenInclude(e => e.Brand)
            .Include(e => e.Site).ThenInclude(e => e.SiteZones)
            .FirstOrDefault(e => e.FingerPrint == machineFingerPrint);

        // if machine exist return it to cache
        if (machineobj != null)
        {
            var token = await MachineLogin(machineobj, machineModel.ConnectionId);
            result.SiteId = machineobj.SiteId;
            result.BrandId = machineobj.Site.BrandId;
            result.Token = token;
            result.ZoneIds = machineobj.Site.SiteZones?.Select(e => e.ZoneId).ToList();

            return Ok(result);
        }
        // if machine not exist, create one and return it to cache
        else
        {
            try
            {
                bool isGuid = Guid.TryParse(machineModel.Hash, out Guid hash);
                if (isGuid)
                {
                    var site = _context.Sites.Include(e => e.SiteZones).FirstOrDefault(e => e.Hash == hash);

                    Machine machine = new Machine()
                    {
                        SiteId = site.Id,
                        FingerPrint = machineFingerPrint,
                        CurrentStatus = MachineStatus.Alive,
                        Name = machineModel.MachineName,
                    };

                    await _context.Machines.AddAsync(machine);
                    await _context.SaveChangesAsync();

                    await AddMachineLogOnLoginLogout
                        (
                        connectionId: machineModel.ConnectionId,
                        machineStatus: MachineStatus.Alive,
                        fingerPrint: machineFingerPrint
                        );

                    var token = machineFingerPrint;

                    result.SiteId = site.Id;
                    result.BrandId = site.BrandId;
                    result.Token = token;
                    result.ZoneIds = site.SiteZones?.Select(e => e.ZoneId).ToList();

                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> OnMachineDisConnect([FromBody] MachineModel machineModel)
    {
        SystemInfo systemGuid = new SystemInfo();
        string machineFingerPrint = machineModel.SystemInfo.EncryptString();

        bool isLogged = await AddMachineLogOnLoginLogout
            (
            connectionId: machineModel.ConnectionId,
            machineStatus: MachineStatus.Closed,
            fingerPrint: machineFingerPrint
            );

        return Ok(isLogged);
    }

    [HttpGet]
    public async Task<List<string>> GetMachinePendingOrdersWhenBeingOnline([FromQuery] int brandId, [FromQuery] int? siteId)
    {
        List<string> orders = new List<string>();

        var machineMessageLogs = _logDbContext.MachineMessageLogs.Where(e => e.BrandId == brandId && e.SiteId == siteId && e.ReceivedAt == null);

        if (machineMessageLogs?.Any() ?? false)
        {
            orders.AddRange(machineMessageLogs.Select(e => e.Payload).ToList());
            await machineMessageLogs.ForEachAsync(e => e.ReceivedAt = DateTime.Now);
        }
        await _logDbContext.SaveChangesAsync();

        return orders;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> OnSendMessagesToMachine([FromBody] MachineMessageLogCommand request)
    {
        var siteZone = _context.SiteZones.Include(e => e.Site).Where(e => e.ZoneId == request.ZoneId).FirstOrDefault(e => e.Site.BrandId == request.BrandId);
        if (siteZone != null)
        {
            MachineMessageLog machineMessageLog = new MachineMessageLog()
            {
                SentAt = request.SentAt,
                ReceivedAt = request.ReceivedAt,
                Payload = request.Payload,
                BrandId = request.BrandId,
                ZoneId = request.ZoneId,
                MachineName = request.MachineName,
                ConnectionId = request.ConnectionId,
                SiteId = siteZone.SiteId,
                SiteHash = siteZone.Site.HashString,
            };
            await _logDbContext.MachineMessageLogs.AddAsync(machineMessageLog);
            await _logDbContext.SaveChangesAsync();
            return Ok(true);
        }

        return Ok(false);
    }

    private async Task<string> MachineLogin(Machine machineobj, string newConnectionId)
    {
        // if registered
        //if (machineobj.CurrentStatus == MachineStatus.Closed)
        //{
        // token is valid
        if (!string.IsNullOrWhiteSpace(machineobj.FingerPrint))
        {

            bool isLogged = await AddMachineLogOnLoginLogout
                (
                connectionId: newConnectionId,
                machineStatus: MachineStatus.Alive,
                fingerPrint: machineobj.FingerPrint
                );

            return machineobj.FingerPrint;
        }
        // if token not valid
        else
        {
            return String.Empty;
        }
        //}

        //return "";
    }

    private async Task<bool> AddMachineLogOnLoginLogout(MachineStatus machineStatus, string fingerPrint, string connectionId)
    {
        Machine dbMachine = _context.Machines
.Include(e => e.Site).ThenInclude(e => e.Brand)
.Include(e => e.Site).ThenInclude(e => e.SiteZones)
.FirstOrDefault(e => e.FingerPrint == fingerPrint);

        if (dbMachine != null)
        {

            dbMachine.CurrentStatus = machineStatus;
            await _context.SaveChangesAsync();

            MachineStatusLog machineLog = new MachineStatusLog()
            {
                MachineId = dbMachine.Id,
                OccurredAt = DateTime.Now,
                Status = machineStatus,
                ConnectionId = connectionId,
                MachineName = dbMachine.Name,
                SiteId = dbMachine.SiteId,
                SiteHash = dbMachine.Site.HashString,
                SiteName = dbMachine.Site.Name,
                BrandId = dbMachine.Site.Brand.Id,
                BrandName = dbMachine.Site.Brand.Name,
            };

            await _logDbContext.MachineStatusLogs.AddAsync(machineLog);
            await _logDbContext.SaveChangesAsync();

            return true;
        }
        else
        {
            // machine not found using fingerPrint!
            return false;
        }
    }

}
