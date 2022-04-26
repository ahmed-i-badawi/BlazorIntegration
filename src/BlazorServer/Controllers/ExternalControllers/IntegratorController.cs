using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BlazorServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using SharedLibrary.Commands;
using SharedLibrary.Entities;

namespace BlazorServer.Controllers
{
    [Authorize]
    public class IntegratorController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        public IConfiguration _config { get; }

        public IntegratorController(IApplicationDbContext context, IConfiguration config)
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

        private Integrator Authenticate(IntegratorModel SiteLogin)
        {
            var idGuid = Guid.TryParse(SiteLogin.Hash, out Guid id);
            if (idGuid)
            {
                var currentIntegrator = _context.Integrators.FirstOrDefault(b => b.Id == id);

                if (currentIntegrator != null)
                {
                    return currentIntegrator;
                }
            }


            return null;
        }

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

        [HttpPost]
        public async Task<ActionResult<bool>> CheckIntegratorAvaiability([FromBody] IntegratorModel model)
        {
            var integratorObj = _context.Integrators.FirstOrDefault(e => e.Id == Guid.Parse(model.Hash));

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