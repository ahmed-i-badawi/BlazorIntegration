﻿using BlazorServer.Data;
using BlazorServer.Data.Entities;
using BlazorServer.Extensions;
using BlazorServer.Services;
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

namespace BlazorServer.Controllers;

[Authorize]
public class MachineController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;
    public IConfiguration _config { get; }

    public MachineController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
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
        var machineobj = _context.Machines.Include(e => e.Site).ThenInclude(e => e.SiteZones).FirstOrDefault(e => e.FingerPrint == machineFingerPrint);

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
                    MachineLogs = new List<MachineLog>()
                {
                    new MachineLog()
                    {
                        OccurredAt = DateTime.Now,
                        Status=MachineStatus.Alive,
                        ConnectionId=machineModel.ConnectionId,
                    }
                }
                };

                _context.Machines.Add(machine);
                _context.SaveChanges();

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

        var machineDisconnected = _context.Machines.FirstOrDefault(m => m.FingerPrint == machineFingerPrint);
        if (machineDisconnected != null)
        {
            // if registered close connection log
            if (machineDisconnected.CurrentStatus == MachineStatus.Alive)
            {
                MachineLog machineLog = new MachineLog()
                {
                    MachineId = machineDisconnected.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Closed,
                    ConnectionId = machineModel.ConnectionId
                };
                _context.MachineLogs.Add(machineLog);
                machineDisconnected.CurrentStatus = MachineStatus.Closed;
            }
            //machineDisconnected.ConnectionId = string.Empty;
            _context.SaveChanges();
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
                ConnectionId = newConnectionId
            };
            _context.MachineLogs.Add(log);
            //machineobj.ConnectionId = newConnectionId;
            machineobj.CurrentStatus = MachineStatus.Alive;
            _context.SaveChanges();
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
