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
using SharedLibrary.Models;

namespace BlazorServer.Controllers;

[Authorize]
public class MachineController : ApiControllerBase
{
    private readonly IApplicationDbContext _context;
    public IConfiguration _config { get; }
    public ILogDbContext _logDbContext { get; }
    public IEmailService _emailService { get; }

    public MachineController(IApplicationDbContext context,
        IConfiguration config,
        ILogDbContext logDbContext,
        IEmailService emailService
        )
    {
        _context = context;
        _config = config;
        _logDbContext = logDbContext;
        _emailService = emailService;
    }

    private async Task SendMachineRegisterationMail(Site dbSite)
    {
        EmailMessageModel mailMessage = new EmailMessageModel(
              "AhmedBadawi127@gmail.com",
              $"machine registeration details",
              $"you have registered your machine: {dbSite.Machine.Name} on your hash: {dbSite.Hash}");

        await _emailService.SendEmail(mailMessage);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> RegisterMachine([FromBody] MachineRegistrationCommand request)
    {
        string message = "";

        if (Guid.TryParse(request.Hash, out Guid guid))
        {
            var dbSite = _context.Sites.Include(e => e.Machine).FirstOrDefault(e => e.Hash == guid);

            if (dbSite != null)
            {
                if (dbSite.Machine != null)
                {
                    message = $"site with hash:  {dbSite.Hash} has a machine with name: {dbSite.Machine.Name}";
                    // here

                    await SendMachineRegisterationMail(dbSite);

                    return Ok(message);
                }
                else
                {
                    // site has no machine, check if machine has same finger print
                    SystemGuid systemGuid = new SystemGuid();
                    var fingerPrint = systemGuid.ValueAsync();
                    Console.WriteLine("finger from controller server after blazor client");
                    Console.WriteLine(fingerPrint);
                    var dbMachine = _context.Machines.FirstOrDefault(e => e.FingerPrint == fingerPrint);

                    if (dbMachine != null)
                    {
                        if (dbMachine.SiteId == null)
                        {
                            // pending => link to site
                            dbMachine.SiteId = dbSite.Id;
                            dbMachine.Name = request.MachineName;
                            dbMachine.CurrentStatus = MachineStatus.Closed;
                            await _context.SaveChangesAsync();

                            message = $"machine: {request.MachineName} is now registered on site with hash: {dbSite.Hash}";
                            //await OnMachineConnect(new MachineModel()
                            //{
                            //    ConnectionId = request.ConnectionId,
                            //    Hash = request.Hash,
                            //    MachineName = request.MachineName,
                            //    SiteId = request.SiteId,
                            //    SystemInfo = request.SystemInfo,
                            //    Notes = request.Notes
                            //});
                        }
                        else
                        {
                            message = $"this machine is allready registered on another site";
                        }
                    }
                    else
                    {

                        message = $"Kindely download installer and start your worker";
                    }

                    return Ok(message);
                }
            }
        }
        message = "No Site hash available";

        return Ok(message);
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
            result.BrandId = machineobj.Site?.BrandId ?? 0;
            result.Token = token;
            result.ZoneIds = machineobj.Site?.SiteZones?.Select(e => e.ZoneId).ToList();

            return Ok(result);
        }
        // if machine not exist, create one and add to db
        else
        {
            try
            {

                Machine machine = new Machine()
                {
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

                //var token = machineFingerPrint;

                return Ok(result);
                //}
                //else
                //{
                //    return NotFound();
                //}
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
                SiteUserId = siteZone.Site?.ApplicationUserId,
                SiteHash = siteZone.Site?.HashString,
            };
            await _logDbContext.MachineMessageLogs.AddAsync(machineMessageLog);
            await _logDbContext.SaveChangesAsync();
            return Ok(true);
        }

        return Ok(false);
    }

    private async Task<string> MachineLogin(Machine machineobj, string newConnectionId)
    {
        // token is valid
        if (!string.IsNullOrWhiteSpace(machineobj.FingerPrint))
        {
            bool isLogged = await AddMachineLogOnLoginLogout
                (
                connectionId: newConnectionId,
                machineStatus: MachineStatus.Alive,
                fingerPrint: machineobj.FingerPrint
                );
            if (machineobj.SiteId == null)
            {
                return String.Empty;
            }
            return machineobj.FingerPrint;
        }
        // if token not valid
        else
        {
            return String.Empty;
        }
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
                SiteUserId = dbMachine.Site?.ApplicationUserId,
                SiteHash = dbMachine.Site?.HashString,
                SiteName = dbMachine.Site?.Name,
                BrandId = dbMachine.Site?.Brand?.Id,
                BrandName = dbMachine.Site?.Brand?.Name,
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
