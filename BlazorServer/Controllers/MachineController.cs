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

namespace BlazorServer.Controllers;

public class MachineController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;
    private IHubContext<MessagingHub> _hubContext { get; }
    public IConfiguration _config { get; }

    public MachineController(IHubContext<MessagingHub> hubContext, ApplicationDbContext context, IConfiguration config)
    {
        _hubContext = hubContext;
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
    public async Task<ActionResult<bool>> ValidateRegister([FromBody] MachineRegistrationCommand command)
    {
        var brandObj = _context.Brands.Include(e => e.Machines).FirstOrDefault(e => e.Hash == command.Hash);

        if (brandObj != null)
        {
            if (brandObj.MaxNumberOfMachines == brandObj.Machines.Count())
            {
                return Ok(false);
            }
            else if (brandObj.Machines.Any(e => e.Name == command.MachineName))
            {
                return Ok(false);
            }
            else
            {
                return Ok(true);
            }
        }
        else
        {
            return Ok(false);
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> RegisterMachine([FromBody] MachineRegistrationCommand command)
    {
        var brandObj = _context.Brands.Include(e => e.Machines).FirstOrDefault(e => e.Hash == command.Hash);

        SystemGuid guid = new SystemGuid();
        var fingerPrint = guid.ValueAsync();

        Machine machine = new Machine()
        {
            Name = command.MachineName,
            DateAdded = DateTime.Now,
            CurrentStatus = MachineStatus.Pending,
            ConnectionId = command.ConnectionId,
            FingerPrint = fingerPrint
        };


        brandObj.Machines.Add(machine);

        _context.SaveChanges();

        var machineObj = _context.Machines.FirstOrDefault(e => e.FingerPrint == fingerPrint);

        if (machineObj != null)
        {
            MachineLog machineLog = new MachineLog()
            {
                MachineId = machineObj.Id,
                OccurredAt = DateTime.Now,
                Status = MachineStatus.Pending
            };
            _context.MachineLogs.Add(machineLog);
        }

        return Ok(true);
    }

    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] SubmitMachineCommand request)
    {
        try
        {
            SystemGuid systemGuid = new SystemGuid();

            string machineFingerPrint = request.SystemInfo.EncryptString();

            var machineobj = _context.Machines.Include(e => e.Brand).First(e => e.FingerPrint == machineFingerPrint);

            // if Machine Exist
            if (machineobj != null)
            {
                // register machine with Pending Status
                if (machineobj.CurrentStatus == MachineStatus.Pending)
                {
                    var isMachineAdded = await SubmitMachineHandler(machineobj, machineFingerPrint);
                    if (isMachineAdded)
                    {
                        return Ok("Submitted Machine Successfully");
                    }
                    else
                    {
                        return NotFound("Submit Machine Failed");
                    }
                }
                // log in machine with active status
                else if (machineobj.CurrentStatus == MachineStatus.Active)
                {

                    var token = await LoggingHandler(machineobj, machineFingerPrint, "1111111111" );

                    // token is valid
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return Ok(token);
                    }
                    // if token not valid
                    else
                    {
                        return NotFound("Machine Not Valid");
                    }
                }
            }

            return NotFound("Machine NotFound");
        }
        catch (Exception ex)
        {
            return NotFound("Database Exception");
        }


    }

    private async Task<string> GenerateToken(string fingerPrint)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.SerialNumber, fingerPrint),
            new Claim(ClaimTypes.Role, "Machine")
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Audience"],
          claims,
          expires: DateTime.Now.AddMinutes(999),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<bool> LoginLog(Machine machineEntity, string connectionId)
    {
        MachineStatus status = MachineStatus.Alive;

        machineEntity.CurrentStatus = status;
        machineEntity.ConnectionId = connectionId;

        MachineLog log = new MachineLog()
        {
            MachineId = machineEntity.Id,
            OccurredAt = DateTime.Now,
            Status = status
        };
        _context.MachineLogs.Add(log);
        _context.SaveChanges();
        return true;
    }
    private async Task<string> LoggingHandler(Machine machineEntity, string fingerPrint, string connectionId)
    {
        var token = await GenerateToken(fingerPrint);

        if (!string.IsNullOrWhiteSpace(token))
        {
            bool isLogsSuccess = await LoginLog(machineEntity, connectionId);
        }

        return token;
    }

    private async Task<bool> SubmitMachineHandler(Machine machineEntity, string fingerPrint)
    {
        if (machineEntity.FingerPrint == fingerPrint)
        {
            await _hubContext.Clients.Client(machineEntity.ConnectionId).SendAsync("MachineIsAdded", "");
            machineEntity.CurrentStatus = MachineStatus.Active;
            machineEntity.ConnectionId = null;
            MachineLog log = new MachineLog()
            {
                MachineId = machineEntity.Id,
                OccurredAt = DateTime.Now,
                Status = MachineStatus.Active
            };
            _context.MachineLogs.Add(log);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
}
