﻿using Infrastructure.ApplicationDatabase.Common.Interfaces;
using System.Security.Claims;


namespace Infrastructure.ApplicationDatabase.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    //public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string UserId { get { return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } }

}
