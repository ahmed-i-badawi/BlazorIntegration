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
            // send all opened connections
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", request);

            //send user by id (no)
            //await _hubContext.Clients.User("daa62ebc-edd9-4efe-8ad0-88c07fd707da").SendAsync("ReceiveMessage", request);

            // to send connectionId
            //await _hubContext.Clients.Client(request.connId).SendAsync("ReceiveMessage", request);

            return Ok("done");
        }
    }
}