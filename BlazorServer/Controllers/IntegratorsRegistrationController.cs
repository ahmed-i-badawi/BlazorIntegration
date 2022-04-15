﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorServer.Data;
using BlazorServer.Data.Entities;
using SharedLibrary.Commands;
using AutoMapper;
using Syncfusion.Blazor;
using BlazorServer.Extensions;
using SharedLibrary.Dto;

namespace BlazorServer.Controllers
{
    public class IntegratorsRegistrationController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public IntegratorsRegistrationController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> GetIntegrators([FromBody] DataManagerRequest dm)
        {
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

        [HttpPut("{id}")]
        public async Task<IActionResult> EditIntegrator(string hash, IntegratorsRegistrationCreateCommand command)
        {
            //if (hash != command.Id)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(integrator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IntegratorExists(hash))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<int>> PostIntegrator(IntegratorsRegistrationCreateCommand command)
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
            return Ok(1);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteIntegrator([FromBody]  string id)
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
            return _context.Integrators.Any(e => e.Hash == id);
        }
    }
}
