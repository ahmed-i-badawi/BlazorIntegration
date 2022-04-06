using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
//using Shared.Commands;
using BlazorServer.Services;
using BlazorServer.Data;
using Microsoft.EntityFrameworkCore;
using BlazorServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BlazorServer.Data.Entities;

namespace BlazorServer.Controllers
{
    public class MessageController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _config { get; }

        public MessageController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string Generate(Brand brand)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
            new Claim("Hash", brand.Hash),
            new Claim("Brand", brand.Id.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(999),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Brand Authenticate(BrandModel brandLogin)
        {
            var currentBrand = _context.Brands.FirstOrDefault(b => b.Hash == brandLogin.Hash);

            if (currentBrand != null)
            {
                return currentBrand;
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] BrandModel brandLogin)
        {
            var user = Authenticate(brandLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("Brand not found");
        }

        //[Authorize(Policy = "MachineToMachine")]
        [HttpPost]
        public async Task<ActionResult<string>> GetMessageStatus([FromBody] int brandId)
        {
            var machineObj = _context.Machines.Include(e => e.Brand).FirstOrDefault(e => e.BrandId == brandId);

            if (machineObj != null)
            {
                return Ok(machineObj?.ConnectionId);
            }
            else
            {
                return NotFound();

            }
        }
    }
}