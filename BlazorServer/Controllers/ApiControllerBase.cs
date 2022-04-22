using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SharedLibrary.Commands;
using System.Security.Claims;

namespace BlazorServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    public string? UserId => HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

}
