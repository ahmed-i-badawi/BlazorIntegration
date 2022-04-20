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

namespace BlazorServer.Controllers;

[Authorize]
public class MachineController : ApiControllerBase
{
    private readonly IApplicationDbContext _context;
    public IConfiguration _config { get; }
    public ILogDbContext _logDbContext { get; }

    public MachineController(IApplicationDbContext context, IConfiguration config, ILogDbContext logDbContext)
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
                var site = _context.Sites.Include(e => e.SiteZones).FirstOrDefault(e => e.Hash == Guid.Parse(machineModel.Hash));

                Machine machine = new Machine()
                {
                    SiteId = site.Id,
                    FingerPrint = machineFingerPrint,
                    CurrentStatus = MachineStatus.Alive,
                    Name = machineModel.MachineName,
                };

                await _context.Machines.AddAsync(machine);
                await _context.SaveChangesAsync();


                var newMachineAdded = _context.Machines
    .Include(e => e.Site).ThenInclude(e => e.Brand)
    .Include(e => e.Site).ThenInclude(e => e.SiteZones)
    .FirstOrDefault(e => e.FingerPrint == machineFingerPrint);

                MachineLog machineLog = new MachineLog()
                {
                    MachineId = newMachineAdded.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Alive,
                    ConnectionId = machineModel.ConnectionId,
                    SiteId = newMachineAdded.SiteId,
                    SiteHash = newMachineAdded.Site.HashString,
                    SiteName = newMachineAdded.Site.Name,
                    BrandId = newMachineAdded.Site.Brand.Id,
                    BrandName = newMachineAdded.Site.Brand.Name,
                };

                await _logDbContext.MachineLogs.AddAsync(machineLog);
                await _logDbContext.SaveChangesAsync();

                var token = machineFingerPrint;

                result.SiteId = site.Id;
                result.BrandId = site.BrandId;
                result.Token = token;
                result.ZoneIds = site.SiteZones?.Select(e => e.ZoneId).ToList();

                return Ok(result);
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

        var machineDisconnected = _context.Machines.Include(e => e.Site).ThenInclude(e => e.Brand).FirstOrDefault(m => m.FingerPrint == machineFingerPrint);
        if (machineDisconnected != null)
        {
            // if registered close connection log
            if (machineDisconnected.CurrentStatus == MachineStatus.Alive)
            {
                MachineLog machineLog = new MachineLog()
                {
                    MachineId = machineDisconnected.Id,
                    MachineName = machineDisconnected?.Name,
                    SiteId = machineDisconnected.SiteId,
                    SiteHash = machineDisconnected.Site.HashString,
                    SiteName = machineDisconnected.Site.Name,
                    BrandId = machineDisconnected.Site.Brand.Id,
                    BrandName = machineDisconnected.Site.Brand.Name,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Closed,
                    ConnectionId = machineModel.ConnectionId
                };
                await _logDbContext.MachineLogs.AddAsync(machineLog);
                machineDisconnected.CurrentStatus = MachineStatus.Closed;
            }
            //machineDisconnected.ConnectionId = string.Empty;
            await _logDbContext.SaveChangesAsync();
            await _context.SaveChangesAsync();
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
            MachineLog log = new MachineLog()
            {
                MachineId = machineobj.Id,
                OccurredAt = DateTime.Now,
                Status = MachineStatus.Alive,
                ConnectionId = newConnectionId,
                MachineName = machineobj.Name,
                SiteId = machineobj.SiteId,
                SiteHash = machineobj.Site.HashString,
                SiteName = machineobj.Site.Name,
                BrandId = machineobj.Site.Brand.Id,
                BrandName = machineobj.Site.Brand.Name,
            };
            await _logDbContext.MachineLogs.AddAsync(log);
            machineobj.CurrentStatus = MachineStatus.Alive;
            await _context.SaveChangesAsync();
            try
            {
                await _logDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
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

}
