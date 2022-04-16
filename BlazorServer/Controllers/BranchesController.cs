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
    public class BranchesController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BranchesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> GetBranches([FromBody] DataManagerRequest dm)
        {
            var query = _context.Branches.Include(e=>e.Brand).Include(e=>e.Machine).AsQueryable();

            query = await query.FilterBy(dm);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<BranchDto> data = _mapper.Map<List<BranchDto>>(query.ToList());
            ResultDto<BranchDto> res = new ResultDto<BranchDto>(data, count);

            return Ok(res);
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(string id)
        {
            var branch = await _context.Branches.FindAsync(Guid.Parse(id));

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> EditBranch(BranchCreateCommand command)
        {
            Branch commnad = _mapper.Map<Branch>(command);
            Branch db = _context.Branches.FirstOrDefault(e => e.Id == Guid.Parse(command.Hash));

            if (db != null)
            {
                db.Name = command.Name;
                db.Notes = command.Notes;
                db.Address = command.Address;
                db.BrandId = command.BrandId;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(command.Hash))
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
        public async Task<ActionResult<bool>> PostBranch(BranchCreateCommand command)
        {
            Branch branch = _mapper.Map<Branch>(command);

            _context.Branches.Add(branch);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BranchExists(branch.Hash))
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
        public async Task<IActionResult> DeleteBranch([FromBody]  string id)
        {
            var branch = await _context.Branches.FindAsync(Guid.Parse(id));
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private bool BranchExists(string id)
        {
            return _context.Branches.Any(e => e.Id == Guid.Parse(id));
        }
    }
}
