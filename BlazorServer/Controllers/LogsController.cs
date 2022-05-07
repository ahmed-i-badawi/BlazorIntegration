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
using Infrastructure.LogDatabase.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Controllers
{
    //[Authorize("CanPurge")]
    public class LogsController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogDbContext _logDbContext;

        public LogsController(IApplicationDbContext context, IMapper mapper, ILogDbContext logDbContext)
        {
            _context = context;
            _mapper = mapper;
            _logDbContext = logDbContext;
        }

        [HttpPost]
        public async Task<ActionResult> GetMachineStatusLogs([FromBody] DataManagerRequest dm)
        {
            var queryString = Request.Query;

            string siteId = queryString["siteId"];

            
            var query = _logDbContext.MachineStatusLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(siteId))
            {
                query = query.Where(e => e.SiteUserId == siteId);
            }

            query = await query.FilterBy(dm);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<MachineStatusLogDto> data = _mapper.Map<List<MachineStatusLogDto>>(query.ToList());
            ResultDto<MachineStatusLogDto> res = new ResultDto<MachineStatusLogDto>(data, count);

            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> GetMachineMessageLogs([FromBody] DataManagerRequest dm)
        {
            var queryString = Request.Query;

            string siteId = queryString["siteId"];

            var query = _logDbContext.MachineMessageLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(siteId))
            {
                query = query.Where(e => e.SiteUserId == siteId);
            }

            query = await query.FilterBy(dm);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<MachineMessageLogDto> data = _mapper.Map<List<MachineMessageLogDto>>(query.ToList());
            ResultDto<MachineMessageLogDto> res = new ResultDto<MachineMessageLogDto>(data, count);

            return Ok(res);
        }

    }
}
