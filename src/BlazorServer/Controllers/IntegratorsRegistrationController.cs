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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlazorServer.Controllers
{

    public class IntegratorsRegistrationController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public IntegratorsRegistrationController(IApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> GetIntegrators([FromBody] DataManagerRequest dm)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

            var query = _context.Integrators.AsQueryable();

            query = await query.FilterBy(dm);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<IntegratorsDto> data = _mapper.Map<List<IntegratorsDto>>(query.ToList());
            ResultDto<IntegratorsDto> res = new ResultDto<IntegratorsDto>(data, count);

            return Ok(res);
        }

        // GET: api/IntegratorsRegistration/5
        [HttpGet("{hash}")]
        public async Task<ActionResult<Integrator>> GetIntegrator(string hash)
        {
            var integrator = await _context.Integrators.FindAsync(hash);

            if (integrator == null)
            {
                return NotFound();
            }

            return integrator;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> EditIntegrator(IntegratorsRegistrationCreateCommand command)
        {
            Integrator integratorCommnad = _mapper.Map<Integrator>(command);
            Integrator integratorDb = _context.Integrators.FirstOrDefault(e => e.Id == Guid.Parse(command.Hash));

            if (integratorDb != null)
            {
                integratorDb.Name = command.Name;
                integratorDb.Notes = command.Notes;
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IntegratorExists(command?.Hash))
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
        public async Task<ActionResult<bool>> PostIntegrator(IntegratorsRegistrationCreateCommand command)
        {
            Integrator integrator = _mapper.Map<Integrator>(command);

            _context.Integrators.Add(integrator);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IntegratorExists(integrator.Hash))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteIntegrator([FromBody] string id)
        {
            var integrator = await _context.Integrators.FindAsync(Guid.Parse(id));
            if (integrator == null)
            {
                return NotFound();
            }

            _context.Integrators.Remove(integrator);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private bool IntegratorExists(string id)
        {
            return _context.Integrators.Any(e => e.Id == Guid.Parse(id));
        }
    }
}
