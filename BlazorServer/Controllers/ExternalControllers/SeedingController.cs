using BlazorServer.Data;
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
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using global::BlazorServer.Controllers;
using SharedLibrary.Entities;

namespace BlazorServer.Controllers;

[AllowAnonymous]
public class SeedingController : ApiControllerBase
{
    private readonly IApplicationDbContext _context;
    public IConfiguration _config { get; }
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SeedingController(IApplicationDbContext context,
        IConfiguration config,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor
)
    {
        _context = context;
        _config = config;
        _userManager = userManager;
        _roleManager = roleManager;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<bool>> Init()
    {
        var user2 = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);


        await DbContextSeed.SeedDefaultUserAsync(_userManager, _roleManager, _identityService);
        await DbContextSeed.SeedSampleDataAsync(_context);

        return Ok(true);
    }

}
