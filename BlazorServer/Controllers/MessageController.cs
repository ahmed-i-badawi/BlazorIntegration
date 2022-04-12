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

        private string Generate(Branch branch)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
            new Claim("Hash", branch.Hash),
            new Claim("Branch", branch.Id.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(999),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Branch Authenticate(BranchModel branchLogin)
        {
            var currentBranch = _context.Branchs.FirstOrDefault(b => b.Hash == branchLogin.Hash);

            if (currentBranch != null)
            {
                return currentBranch;
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] BranchModel branchLogin)
        {
            var user = Authenticate(branchLogin);

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
            // toDo
            // get connection from cached list

            return Ok();
            // -----old-------
            //var machineObj = _context.Machines.Include(e => e.Branch).FirstOrDefault(e => e.BrandId == brandId);

            //if (machineObj != null)
            //{
            //    return Ok(machineObj?.ConnectionId);
            //}
            //else
            //{
            //    return NotFound();

            //}
        }
    }
}