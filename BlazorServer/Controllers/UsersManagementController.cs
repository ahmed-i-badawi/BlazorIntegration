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
using SharedLibrary.Enums;
using BlazorServer.Services;
using SharedLibrary.Constants;

namespace BlazorServer.Controllers
{
    public class UsersManagementController : ApiControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public UsersManagementController(IApplicationDbContext context, IMapper mapper,
            IIdentityService identityService, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _emailService = emailService;
        }

        [HttpGet]
        public object GetSitesDropDownList([FromQuery] string Name)
        {
            var query = _context.Sites.Where(e => e.ApplicationUserId == null).AsQueryable().AsNoTracking();
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

            List<SiteDto> data = new List<SiteDto>();

            if (selectedId > 0)
            {
                var selectedObj = query.FirstOrDefault(x => x.Id == selectedId);

                query = query.Take(take - 1);

                var dbData = query.ToList();
                dbData.Insert(0, selectedObj);
                data = _mapper.Map<List<SiteDto>>(dbData);
            }
            else
            {
                query = query.Take(take);
                data = _mapper.Map<List<SiteDto>>(query.ToList());
            }


            return data;
        }


        [HttpGet]
        public object GetIntegratorsDropDownList([FromQuery] string Name)
        {
            var query = _context.Integrators.Where(e=>e.ApplicationUserId == null).AsQueryable().AsNoTracking();
            var queryString = Request.Query;

            int.TryParse(queryString["take"], out int take);
            string selectedId = queryString["selectedId"];
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

            List<IntegratorsDto> data = new List<IntegratorsDto>();

            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                var selectedObj = query.FirstOrDefault(x => x.Hash == selectedId);

                query = query.Take(take - 1);

                var dbData = query.ToList();
                dbData.Insert(0, selectedObj);
                data = _mapper.Map<List<IntegratorsDto>>(dbData);
            }
            else
            {
                query = query.Take(take);
                data = _mapper.Map<List<IntegratorsDto>>(query.ToList());
            }


            return data;
        }

        [HttpPost]
        public async Task<ActionResult> GetUsers([FromBody] DataManagerRequest dm)
        {
            var res = await _identityService.GetUsers(dm); ;

            return Ok(res);
        }


        [HttpPost]
        public async Task<ActionResult> PostUser(UserCreateCommand command)
        {

            var result = await _identityService.CreateUserAsync(
                userName: command.UserName,
                password: command.Password,
                mail: command.Email,
                isActive: command.IsActive,
                userType: (UserType)command.UserType,
                siteId: command.SiteId,
                integratorId: command.IntegratorHash
                );

            if (!result.Result.Succeeded)
            {
                return Ok(false);
            }

            if (command.IsSendMail)
            {
                string hash = "";
                if (command.UserType == (int)UserType.Integrator)
                {
                    hash = (await _context.Integrators.FirstOrDefaultAsync(e => e.ApplicationUserId == result.UserId)).Hash;
                    await _identityService.AddUserToRole(result.UserId, RolesConstants.Integrator);
                    await _emailService.SendIntegratorRegisterationMail(command.Email, hash, command.UserName, command.Password);
                }
                else if (command.UserType == (int)UserType.Site)
                {
                    hash = (await _context.Sites.FirstOrDefaultAsync(e => e.ApplicationUserId == result.UserId)).HashString;
                    await _identityService.AddUserToRole(result.UserId, RolesConstants.Site);
                    await _emailService.SendSiteRegisterationMail(command.Email, hash, command.UserName, command.Password);
                }
            }

            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] string id)
        {
            var result = await _identityService.DeleteUserAsync(id, true);

            if (!result.Succeeded)
            {
                return Ok(false);
            }

            return Ok(true);
        }

    }
}
