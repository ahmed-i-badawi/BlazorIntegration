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
using BlazorServer.Services;

namespace BlazorServer.Controllers
{
    public class LookupController : ApiControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(IMapper mapper, ILookupService lookupService)
        {
            _lookupService = lookupService;
        }


        [HttpGet]
        public async Task<ActionResult> GetLookup([FromQuery] GetLookupQuery query)
        {
            var result = await _lookupService.GetLookup(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
