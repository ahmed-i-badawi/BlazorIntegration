using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;
using BlazorServer.Services;
using BlazorServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Controllers
{
    //[Authorize]
    public class MessageController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private IHubContext<MessagingHub> _hubContext { get; }

        public MessageController(IHubContext<MessagingHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        //[Authorize]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> GetMessageStatus([FromBody] MessageCommand request)
        {
            var machineObj = _context.Machines.Include(e=>e.Brand).FirstOrDefault(e => e.BrandId == request.BrandId);
          
            if (machineObj != null)
            {
                await _hubContext.Clients.Client(machineObj.ConnectionId).SendAsync("NewOrder", "");
                return Ok("done");
            }

            return NotFound("NotFoundddddddddd");

        }
    }
}