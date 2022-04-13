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
using Shared.Commands;

namespace BlazorServer.Controllers
{
    [Authorize(Policy = "MachineToMachine")]
    public class IntegratorController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _config { get; }

        public IntegratorController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string Generate(Integrator integrator)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
            new Claim("Hash", integrator.Hash),
            new Claim("INTEGRATOR", integrator.Id.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(999),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Integrator Authenticate(IntegratorModel branchLogin)
        {
            var currentIntegrator = _context.Integrators.FirstOrDefault(b => b.Hash == branchLogin.Hash);

            if (currentIntegrator != null)
            {
                return currentIntegrator;
            }

            return null;
        }

        [Authorize(Policy = "MachineToMachine")]
        [HttpPost]
        public IActionResult Login([FromBody] IntegratorModel integratorLogin)
        {
            var integrator = Authenticate(integratorLogin);

            if (integrator != null)
            {
                var token = Generate(integrator);
                return Ok(token);
            }

            return NotFound("Integrator not found");
        }

        [Authorize(Policy = "MachineToMachine")]
        [HttpPost]
        public async Task<ActionResult<bool>> CheckIntegratorAvaiability([FromBody] IntegratorModel model)
        {
           var integratorObj = _context.Integrators.FirstOrDefault(e => e.Hash == model.Hash);

            if (integratorObj != null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
    }
}