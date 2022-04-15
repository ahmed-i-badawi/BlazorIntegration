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
using Microsoft.AspNetCore.Identity;

namespace BlazorServer.Controllers;

public class SeedingController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;
    public IConfiguration _config { get; }
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedingController(ApplicationDbContext context, IConfiguration config, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _config = config;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<bool>> Init()
    {
        await ApplicationDbContextSeed.SeedDefaultUserAsync(_userManager, _roleManager);
        await ApplicationDbContextSeed.SeedSampleDataAsync(_context);

        return Ok(true);
    }

}
