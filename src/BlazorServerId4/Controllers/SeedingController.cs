﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BlazorServerId4.Controllers;

namespace BlazorServerId4.Controllers
{

    public class SeedingController : ApiControllerBase
    { 

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult> Init(object ttt)
        {
           var userId = User.Claims;

            return Ok(true);
        }
    }

}