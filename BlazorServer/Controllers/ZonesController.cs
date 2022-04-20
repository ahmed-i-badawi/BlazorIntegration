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
    public class ZonesController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ZonesController(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public object GetZonesDropDownList([FromQuery] string Name)
        {
            var query = _context.Zones.AsQueryable().AsNoTracking();
            var queryString = Request.Query;

            int.TryParse(queryString["take"], out int take);
            int.TryParse(queryString["selectedId"], out int selectedId);
            string filter = queryString["$filter"];

            if (filter != null) // to handle filter opertaion 
            {
                var newfiltersplits = filter;
                var filtersplits = newfiltersplits.Split('(', ')', ' ', '\'');
                var filterfield = filtersplits[2];
                var filtervalue = filtersplits[4];

                if (filtersplits.Length == 7)
                {
                    if (filtersplits[2] == "tolower")
                    {
                        filterfield = filter.Split('(', ')', '\'')[3];
                        filtervalue = filter.Split('(', ')', '\'')[5];
                    }
                }
                switch (filterfield)
                {
                    case "Name":
                        query = query.Where(x => x.Name.ToLower().Contains(filtervalue.ToString()));
                        break;
                }
            }

            List<ZoneDto> data = new List<ZoneDto>();

            if (selectedId > 0)
            {
                var selectedObj = query.FirstOrDefault(x => x.Id == selectedId);

                query = query.Take(take - 1);

                var dbData = query.ToList();
                dbData.Insert(0, selectedObj);
                data = _mapper.Map<List<ZoneDto>>(dbData);
            }
            else
            {
                query = query.Take(take);
                data = _mapper.Map<List<ZoneDto>>(query.ToList());
            }



            return data;
        }

        [HttpPost]
        public async Task<ActionResult> GetZones([FromBody] DataManagerRequest dm)
        {
            var query = _context.Zones.AsQueryable();

            query = await query.FilterBy(dm);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);
            List<ZoneDto> data = _mapper.Map<List<ZoneDto>>(query.ToList());
            ResultDto<ZoneDto> res = new ResultDto<ZoneDto>(data, count);

            return Ok(res);
        }

        // GET: api/Zones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Zone>> GetZone(int id)
        {
            var Zone = await _context.Zones.FindAsync(id);

            if (Zone == null)
            {
                return NotFound();
            }

            return Zone;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> EditZone(ZoneCreateCommand command)
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
        public async Task<ActionResult<bool>> PostZone(ZoneCreateCommand command)
        {
            Zone Zone = _mapper.Map<Zone>(command);

            _context.Zones.Add(Zone);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ZoneExists(Zone.Id))
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
        public async Task<IActionResult> DeleteZone([FromBody]  int id)
        {
            var Zone = await _context.Zones.FindAsync(id);
            if (Zone == null)
            {
                return NotFound();
            }

            _context.Zones.Remove(Zone);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private bool ZoneExists(int id)
        {
            return _context.Zones.Any(e => e.Id == id);
        }
    }
}
