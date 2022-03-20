using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;
using WebAPi.Services;

namespace WebAPi.Controllers
{
    public class MessageController : ApiControllerBase
    {
        private IHubContext<MessagingHub> _hubContext { get; }

        public MessageController(IHubContext<MessagingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<string>> GetMessageStatus([FromBody] MessageCommand request)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", request);

            return Ok("done");
        }
    }
}