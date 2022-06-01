#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Commands;
using AutoMapper;
using Syncfusion.Blazor;
using BlazorServer.Extensions;
using SharedLibrary.Dto;
using Infrastructure.ApplicationDatabase.Services;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using SharedLibrary.Entities;

namespace BlazorServer.Controllers
{
    public class UsersManagementController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        public UsersManagementController(IApplicationDbContext context, IMapper mapper, IIdentityService identityService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
        }
    
        [HttpPost]
        public async Task<ActionResult> GetUsers([FromBody] DataManagerRequest dm)
        {
            var res = await _identityService.GetUsers(dm); ;

            return Ok(res);
        }

        // GET: api/Zones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Zone>> GetUser(int id)
        {
            var Zone = await _context.Zones.FindAsync(id);

            if (Zone == null)
            {
                return NotFound();
            }

            return Zone;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> EditUser(ZoneCreateCommand command)
        {
            Zone commnad = _mapper.Map<Zone>(command);
            Zone db = _context.Zones.FirstOrDefault(e => e.Id == command.Id);

            if (db != null)
            {
                db.Name = command.Name;
                db.Notes = command.Notes;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZoneExists(command.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(true);
        }

        [HttpPost]
        public async Task<ActionResult> PostUser(UserCreateCommand command)
        {
            var result = await _identityService.CreateUserAsync(
                userName: command.UserName,
                password: command.Password,
                mail: command.Email,
                isActive: command.IsActive
                );

            if (!result.Result.Succeeded)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody]  string id)
        {
            var result = await _identityService.DeleteUserAsync(id);

            if (!result.Succeeded)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        private bool ZoneExists(int id)
        {
            return _context.Zones.Any(e => e.Id == id);
        }
    }
}
