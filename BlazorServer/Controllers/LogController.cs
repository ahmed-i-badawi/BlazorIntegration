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
using BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.LogDatabase.Common.Interfaces;

namespace BlazorServer.Controllers
{
    [AllowAnonymous]
    public class LogController : ApiControllerBase
    {
        private readonly ILogDbContext _context;
        private readonly IMapper _mapper;

        public LogController(ILogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> GetTest([FromBody] DataManagerRequest dm)
        {

            var query2 = _context.TestLogs.AsQueryable();


            return Ok(query2);
        }
    }
}
