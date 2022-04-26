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
using Microsoft.AspNetCore.Identity;

namespace BlazorServer.Controllers
{
    //[Authorize("CanPurge")]
    public class LogsController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogDbContext _logDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public LogsController(IApplicationDbContext context, IMapper mapper, ILogDbContext logDbContext,
            IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _logDbContext = logDbContext;
        _userManager = userManager;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;

        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost]
        public async Task<ActionResult> GetMachineStatusLogs([FromBody] DataManagerRequest dm, [FromQuery] string siteId)
        {
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
        public async Task<ActionResult> GetMachineMessageLogs([FromBody] DataManagerRequest dm, [FromQuery] string siteId)
        {
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
