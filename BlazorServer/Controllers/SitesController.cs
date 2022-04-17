#nullable disable
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
    public class SitesController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SitesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> GetSites([FromBody] DataManagerRequest dm)
        {
            var query = _context.Sites.AsQueryable();

            query = await query.FilterBy(dm);
            query = query.Include(e => e.Brand).Include(e => e.Machine);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<SiteDto> data = _mapper.Map<List<SiteDto>>(query.ToList());
            ResultDto<SiteDto> res = new ResultDto<SiteDto>(data, count);

            return Ok(res);
        }

        // GET: api/Sites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Site>> GetSite(int id)
        {
            var Site = await _context.Sites.FindAsync(id);

            if (Site == null)
            {
                return NotFound();
            }

            return Site;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> EditSite(SiteCreateCommand command)
        {
            Site commnad = _mapper.Map<Site>(command);
            Site db = _context.Sites.FirstOrDefault(e => e.Id == command.Id);

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
                if (!SiteExists(command.Id))
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
        public async Task<ActionResult<bool>> PostSite(SiteCreateCommand command)
        {
            Site Site = _mapper.Map<Site>(command);

            _context.Sites.Add(Site);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SiteExists(Site.Id))
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
        public async Task<IActionResult> DeleteSite([FromBody]  int id)
        {
            var Site = await _context.Sites.FindAsync(id);
            if (Site == null)
            {
                return NotFound();
            }

            _context.Sites.Remove(Site);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private bool SiteExists(int id)
        {
            return _context.Sites.Any(e => e.Id == id);
        }
    }
}
