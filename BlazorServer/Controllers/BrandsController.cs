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
    public class BrandsController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BrandsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> GetBrands([FromBody] DataManagerRequest dm)
        {
            var query = _context.Brands.AsQueryable();

            query = await query.FilterBy(dm);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<BrandDto> data = _mapper.Map<List<BrandDto>>(query.ToList());
            ResultDto<BrandDto> res = new ResultDto<BrandDto>(data, count);

            return Ok(res);
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditBrand(int id, BrandCreateCommand command)
        {
            //if (id != command.Id)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
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
        public async Task<ActionResult<int>> PostBrand(BrandCreateCommand command)
        {
            Brand brand = _mapper.Map<Brand>(command);

            _context.Brands.Add(brand);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BrandExists(brand.Id))
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
        public async Task<IActionResult> DeleteBrand([FromBody]  string id)
        {
            var brand = await _context.Brands.FindAsync(Guid.Parse(id));
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
