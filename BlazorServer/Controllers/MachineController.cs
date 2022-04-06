using BlazorServer.Data;
using BlazorServer.Data.Entities;
using BlazorServer.Extensions;
using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Commands;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlazorServer.Extensions;

namespace BlazorServer.Controllers;


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
            List<Brand> brands = new List<Brand>()
        {
            new Brand()
            {
                Id = 1,
                Hash = "uyhdjnfuirdfjkruhjdn",
                MaxNumberOfMachines = 10,
                Name = "Brand1"
            },new Brand()
            {
                Id = 2,
                Hash = "4rfteddsf",
                MaxNumberOfMachines = 4,
                Name = "Brand2"
            },new Brand()
            {
                Id = 3,
                Hash = "sdfgerfgregrg",
                MaxNumberOfMachines = 6,
                Name = "Brand3"
            },new Brand()
            {
                Id = 4,
                Hash = "dfgergdrfghsdfa2334",
                MaxNumberOfMachines = 5,
                Name = "Brand4"
            },new Brand()
            {
                Id = 5,
                Hash = "32rewsdc3evdxv",
                MaxNumberOfMachines = 1,
                Name = "Brand5"
            },new Brand()
            {
                Id = 6,
                Hash = "467yrtgdtrefdfsdf",
                MaxNumberOfMachines = 2,
                Name = "Brand6"
            },new Brand()
            {
                Id = 7,
                Hash = "uyhdjnfuirdfjkruhjdn",
                MaxNumberOfMachines = 22,
                Name = "Brand7"
            },
        };

            _context.Brands.AddRange(brands);
            _context.SaveChanges();
        }

        if (!_context.Branchs?.Any() ?? false)
        {
            List<Branch> branches = new List<Branch>()
            {
                new Branch()
                {
                    Id = 1,
                    Name = "Branch01",
                    Address = "AddressBranch01",
                    BrandId = 1,
                },new Branch()
                {
                    Id = 2,
                    Name = "Branch02",
                    Address = "AddressBranch02",
                    BrandId = 1,
                },new Branch()
                {
                    Id = 3,
                    Name = "Branch03",
                    Address = "AddressBranch0222",
                    BrandId = 1,
                },new Branch()
                {
                    Id = 4,
                    Name = "Branch01",
                    Address = "AddressBranch0333",
                    BrandId = 2,
                },new Branch()
                {
                    Id = 5,
                    Name = "Branch02",
                    Address = "AddressBranch01",
                    BrandId = 1,
                },new Branch()
                {
                    Id = 6,
                    Name = "Branch01",
                    Address = "AddressBranch01",
                    BrandId = 3,
                },new Branch()
                {
                    Id = 7,
                    Name = "Branch01",
                    Address = "AddressBranch01",
                    BrandId = 4,
                },new Branch()
                {
                    Id = 8,
                    Name = "Branch02",
                    Address = "AddressBranch01",
                    BrandId = 4,
                },new Branch()
                {
                    Id = 9,
                    Name = "Branch01",
                    Address = "AddressBranch01",
                    BrandId = 5,
                },new Branch()
                {
                    Id = 10,
                    Name = "Branch02",
                    Address = "AddressBranch01",
                    BrandId = 5,
                },
            };
            _context.Branchs.AddRange(branches);
            _context.SaveChanges();
        }

        return Ok(true);
    }
    [HttpPost]
    public async Task<ActionResult<string>> RegisterMachine([FromBody] MachineRegistrationCommand command)
    {
        var brandObj = _context.Brands.Include(e => e.Machines).FirstOrDefault(e => e.Hash == command.Hash);

        if (brandObj != null)
        {
            SystemGuid systemGuid = new SystemGuid();

            var machineFingerPrint = systemGuid.ValueAsync();

            var machineObj = _context.Machines.FirstOrDefault(e => e.FingerPrint == machineFingerPrint && e.CurrentStatus == MachineStatus.Pending);

            if (machineObj != null)
            {
                machineObj.Name = command.MachineName;
                machineObj.BrandId = brandObj.Id;
                machineObj.CurrentStatus = MachineStatus.Approved;

                MachineLog machineLog = new MachineLog()
                {
                    MachineId = machineObj.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Approved,
                };
                _context.MachineLogs.Add(machineLog);
                _context.SaveChanges();
                return Ok(machineObj.ConnectionId);
            }
            return Ok(false);
        }
        return Ok(false);
    }
    [HttpPost]
    public async Task<ActionResult<bool>> OnMachineConnect([FromBody] MachineModel machineModel)
    {
        SystemInfo systemGuid = new SystemInfo();

        string machineFingerPrint = machineModel.sysInfo.EncryptString();
        var machineobj = _context.Machines.FirstOrDefault(e => e.FingerPrint == machineFingerPrint);

        if (machineobj != null)
        {
            var token = await RegisterIfNotThenLogin(machineobj, machineModel.ConnectionId);
            return Ok(true);
        }
        else
        {
            try
            {
                Machine machine = new Machine()
                {
                    FingerPrint = machineFingerPrint,
                    CurrentStatus = MachineStatus.Pending,
                    ConnectionId = machineModel.ConnectionId,
                    MachineLogs = new List<MachineLog>()
                {
                    new MachineLog()
                    {
                        OccurredAt = DateTime.Now,
                        Status=MachineStatus.Pending,
                    }
                }
                };

                _context.Machines.Add(machine);
                _context.SaveChanges();

                var newMachineobj = _context.Machines.First(e => e.FingerPrint == machineFingerPrint);

                var token = await RegisterIfNotThenLogin(newMachineobj, machineModel.ConnectionId);
                return Ok(true);
            }
            catch (Exception)
            {
                return Ok(false

                    );
            }

        }
    }
    [HttpPost]
    public async Task<ActionResult<bool>> OnMachineDisConnect([FromBody] MachineModel machineModel)
    {
        SystemInfo systemGuid = new SystemInfo();
        string machineFingerPrint = machineModel.sysInfo.EncryptString();

        var machineDisconnected = _context.Machines.FirstOrDefault(m => m.FingerPrint == machineFingerPrint);
        if (machineDisconnected != null)
        {
            // if registered close connection log
            if (machineDisconnected.CurrentStatus != MachineStatus.Pending)
            {
                MachineLog machineLog = new MachineLog()
                {
                    MachineId = machineDisconnected.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Closed
                };
                _context.MachineLogs.Add(machineLog);
                machineDisconnected.CurrentStatus = MachineStatus.Closed;
            }
            machineDisconnected.ConnectionId = string.Empty;
            _context.SaveChanges();
            return Ok(true);
        }
        return Ok(false);
    }
    private async Task<string> RegisterIfNotThenLogin(Machine machineobj, string newConnectionId)
    {
        // if not registered yet
        if (machineobj.CurrentStatus == MachineStatus.Pending)
        {
            // register current machine then login and stuaus to alive
            // need to wait server for registeration
            machineobj.ConnectionId = newConnectionId;
            _context.SaveChanges();
            return String.Empty;
        }
        // if registered
        else if (machineobj.CurrentStatus == MachineStatus.Closed || machineobj.CurrentStatus == MachineStatus.Approved)
        {
            // token is valid
            if (!string.IsNullOrWhiteSpace(machineobj.FingerPrint))
            {
                MachineLog log = new MachineLog()
                {
                    MachineId = machineobj.Id,
                    OccurredAt = DateTime.Now,
                    Status = MachineStatus.Alive
                };
                _context.MachineLogs.Add(log);
                machineobj.ConnectionId = newConnectionId;
                machineobj.CurrentStatus = MachineStatus.Alive;
                _context.SaveChanges();
                return machineobj.FingerPrint;
            }
            // if token not valid
            else
            {
                return String.Empty;
            }
        }

        return "";
    }

}
