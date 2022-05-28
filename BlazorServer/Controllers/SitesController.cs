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
using BlazorServer.Exceptions;
using Infrastructure.ApplicationDatabase.Services;
using Infrastructure.ApplicationDatabase.Common.Interfaces;
using SharedLibrary.Entities;
using SharedLibrary.Models;
using SharedLibrary.Constants;
using BlazorServer.Services;

namespace BlazorServer.Controllers
{
    public class SitesController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public SitesController(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _emailService = emailService;
        }
        private async Task GetSiteZones(List<SiteDto> dto, List<Site> db)
        {
            foreach (var siteEntity in db)
            {
                dto.FirstOrDefault(e => e.Id == siteEntity.Id).Zones = siteEntity.SiteZones?.Select(e => e.Zone).Select(e => new ZoneDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Notes = e.Notes,
                }).ToList();
            }
        }
        [HttpPost]
        public async Task<ActionResult> GetSiteZones([FromBody] DataManagerRequest dm)
        {
            int.TryParse(dm.Params.FirstOrDefault(e => e.Key == "siteId").Value.ToString(), out int siteId);

            var query2 = _context.Sites.Include(e => e.SiteZones).ThenInclude(e => e.Zone).AsQueryable();


            var query = query2.FirstOrDefault(e => e.Id == siteId).SiteZones.AsQueryable();

            //var query = _context.Zones.Include(e => e.SiteZones).ThenInclude(e => e.Site).AsQueryable();

            //if (siteId > 0)
            //{
            //    query = query.Where(e => e.Id == siteId).AsQueryable();
            //}

            query = await query.FilterBy(dm);
            //query = query.Include(e => e.SiteZones).ThenInclude(e => e.Zone);
            int count = query.Count();
            query = await query.PageBy(dm);

            List<SiteZone> qList = query.ToList();
            List<SiteZoneDto> data = _mapper.Map<List<SiteZoneDto>>(qList);

            //await GetSiteZones(data, qList);

            ResultDto<SiteZoneDto> res = new ResultDto<SiteZoneDto>(data, count);

            return Ok(res);
        }


        [HttpPost]
        public async Task<ActionResult> GetSites([FromBody] DataManagerRequest dm)
        {
            var query = _context.Sites.AsQueryable();

            query = await query.FilterBy(dm);
            query = query.Include(e => e.Brand).Include(e => e.Machines)
                .Include(e => e.SiteZones).ThenInclude(e => e.Zone).Include(e => e.ApplicationUser);
            int count = await query.CountAsync();
            query = await query.PageBy(dm);

            List<Site> qList = query.ToList();
            List<SiteDto> data = _mapper.Map<List<SiteDto>>(qList);

            await GetSiteZones(data, qList);

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
            bool? isPasswordAdded = null;
            if (!string.IsNullOrWhiteSpace(command.Password))
            {
                isPasswordAdded = await _identityService.SetUserPasswrod(command.ApplicationUserId, command.Password);
            }

            Site commnad = _mapper.Map<Site>(command);
            Site db = _context.Sites.FirstOrDefault(e => e.Id == command.Id);

            if (db != null)
            {
                if (command.MaxNumberOfMachines < db.ActualNumberOfMachines)
                {
                    return Ok(false);
                }

                db.Name = command.Name;
                db.Notes = command.Notes;
                db.Address = command.Address;
                db.BrandId = command.BrandId;
                db.MaxNumberOfMachines = command.MaxNumberOfMachines;
            }

            await _context.SaveChangesAsync();
            return Ok(true);

            return Ok(false);
        }



        [HttpPost]
        public async Task<ActionResult<bool>> PostSite(SiteCreateCommand command)
        {
            bool isUserExist = await _identityService.IsUserNameOrMailExist(command.UserName, command.Email);
            if (isUserExist)
            {
                return Ok(false);
            }
            // createUser
            var user = await _identityService.CreateUserAsync(command.UserName, command.Password, true, command.Email);

            if (user.Result.Succeeded)
            {
                await _identityService.AddUserToRole(user.UserId, RolesConstants.Site);

                // createSite
                command.ApplicationUserId = user.UserId;
                Site Site = _mapper.Map<Site>(command);

                _context.Sites.Add(Site);
                await _context.SaveChangesAsync();

                await _emailService.SendSiteRegisterationMail(Site);

                return Ok(true);
            }

            return Ok(false);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteSite([FromBody] int id)
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

        // SiteZones handler

        [HttpPost]
        public async Task<IActionResult> DeleteSiteZone(DeleteSiteZoneCommand command)
        {
            var Site = _context.Sites.Include(e => e.SiteZones).FirstOrDefault(e => e.Id == command.SiteId);
            if (Site == null)
            {
                return NotFound();
            }

            var SiteZone = Site.SiteZones.FirstOrDefault(e => e.ZoneId == command.ZoneId);
            Site.SiteZones.Remove(SiteZone);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> PostSiteZone(SiteZoneCreateCommand command)
        {
            var site = _context.Sites.Include(e => e.SiteZones).FirstOrDefault(e => e.Id == command.SiteId);
            if (site == null)
            {
                return NotFound();
            }
            if (site.SiteZones.Any(e => e.ZoneId == command.ZoneId))
            {
                return NotFound($"Zone {command.ZoneId} is exist on {site.Name}");
            }
            var siteZone = new SiteZone()
            {
                SiteId = command.SiteId,
                ZoneId = command.ZoneId,
                Notes = command.Notes,
            };

            site.SiteZones?.Add(siteZone);

            int res = await _context.SaveChangesAsync();

            return Ok(res.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> EditSiteZone(SiteZoneCreateCommand command)
        {
            var site = _context.Sites.Include(e => e.SiteZones).FirstOrDefault(e => e.Id == command.SiteId);
            if (site == null)
            {
                return NotFound();
            }

            var siteZone = site.SiteZones.FirstOrDefault(e => e.ZoneId == command.ZoneId);
            siteZone.Notes = command.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SiteZoneExists(command.ZoneId))
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

        private bool SiteZoneExists(int id)
        {
            return _context.Sites.Include(e => e.SiteZones).FirstOrDefault(e => e.Id == id).SiteZones.Any(e => e.ZoneId == id);
        }

        private bool SiteExists(int id)
        {
            return _context.Sites.Any(e => e.Id == id);
        }
    }
}
