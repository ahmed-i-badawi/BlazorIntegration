#nullable disable
using AutoMapper;
using BlazorServer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Commands;
using SharedLibrary.Dto;
using Syncfusion.Blazor;
using Infrastructure.ApplicationDatabase.Services;
using BlazorServer.Services;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using SharedLibrary.Entities;

namespace BlazorServer.Controllers
{
    public class BrandsController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BrandsController(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public object GetBrandsDropDownList([FromQuery] string Name)
        {
            var query = _context.Brands.AsQueryable().AsNoTracking();
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

            List<BrandDto> data = new List<BrandDto>();

            if (selectedId > 0)
            {
                var selectedObj = query.FirstOrDefault(x => x.Id == selectedId);

                query = query.Take(take - 1);

                var dbData = query.ToList();
                dbData.Insert(0, selectedObj);
                data = _mapper.Map<List<BrandDto>>(dbData);
            }
            else
            {
                query = query.Take(take);
                data = _mapper.Map<List<BrandDto>>(query.ToList());
            }



            return data;
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

        [HttpPost]
        public async Task<ActionResult<bool>> EditBrand(BrandCreateCommand command)
        {
            Brand integratorCommnad = _mapper.Map<Brand>(command);
            Brand integratorDb = _context.Brands.FirstOrDefault(e => e.Id == command.Id);

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
                if (!BrandExists(command.Id.Value))
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
        public async Task<ActionResult<bool>> PostBrand(BrandCreateCommand command)
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
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand([FromBody] int id)
        {
            var brand = await _context.Brands.FindAsync(id);
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
