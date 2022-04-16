using BlazorServer.Data;
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

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<bool> Seeding()
    {
        if (!_context.Brands?.Any() ?? false)
        {
            var brands = Enumerable.Range(1, 100).Select(x => new Brand()
            {
                Name = $"Brand{x}",
                Notes = $"this is brand{x} Notes"
            });

            _context.Brands.AddRange(brands);
            _context.SaveChanges();
        }

        if (!_context.Branches?.Any() ?? false)
        {
            var branches = Enumerable.Range(1, 2500).Select(x => new Branch()
            {
                Name = $"Branch{x}",
                Address = (new string[] { "Cairo", "Giza", "Alex", "USA", "KSA" })[new Random().Next(5)],
                Notes = $"this is branch{x} Notes",
                BrandId = new Random().Next(1, 100),
            });

            _context.Branches.AddRange(branches);
            _context.SaveChanges();
        }

        if (!_context.Integrators?.Any() ?? false)
        {
            var integrators = Enumerable.Range(1, 500).Select(x => new Integrator()
            {
                Name = $"integrator{x}",
                Notes = $"this is integrator{x} Notes"
            });

            _context.Integrators.AddRange(integrators);
            _context.SaveChanges();
        }


        return Ok(true);
    }

    [HttpPost]
    public async Task<ActionResult<HashCheckerDto>> HashChecker([FromBody] string hash)
    {
        HashCheckerDto res = new HashCheckerDto();

        bool isGuid = Guid.TryParse(hash, out Guid id);

        if (isGuid)
        {
            var brandObj = _context.Branches.Include(e => e.Machine).FirstOrDefault(e => e.Id == id);

            if (brandObj != null)
            {
                if (brandObj.Machine != null)
                {
                    res.SystemInfo = string.Empty;
                    res.Message = "this branch has a machine";

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
        var machineobj = _context.Machines.Include(e => e.Branch).FirstOrDefault(e => e.FingerPrint == machineFingerPrint);

        // if machine exist
        if (machineobj != null)
        {
            var token = await MachineLogin(machineobj, machineModel.ConnectionId);
            result.BranshId = machineobj.BranchId.ToString();
            result.BrandId = machineobj.Branch.BrandId;
            result.Token = token;

            return Ok(result);
        }
        // if machine not exist, create one
        else
        {
            try
            {
                var branch = _context.Branches.FirstOrDefault(e => e.Id == Guid.Parse(machineModel.Hash));

                Machine machine = new Machine()
                {
                    BranchId = branch.Id,
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

                result.BranshId = branch.Id.ToString();
                result.BrandId = branch.BrandId;
                result.Token = token;

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
