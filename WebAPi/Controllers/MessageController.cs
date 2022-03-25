using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;
using WebAPi.Services;

namespace WebAPi.Controllers
{
    [Authorize]
    public class MessageController : ApiControllerBase
    {
        private IHubContext<MessagingHub> _hubContext { get; }

        public MessageController(IHubContext<MessagingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> GetMessageStatus([FromBody] MessageCommand request)
        {

            bool isExist = UserHandler.Connections.Any(e => e.BrandId == request.BrandId && e.BranchId == request.BranchId);
          

            if (isExist)
            {

            }
            //else
            //{
            //    // new Connection
            //    UserHandler.Connections.Add(request.connId, request.StatusId);
            //}

            // to send connectionId
            await _hubContext.Clients.Client(request.connId).SendAsync("ReceiveMessage", request);

            //send user by id (no)
            //await _hubContext.Clients.User("daa62ebc-edd9-4efe-8ad0-88c07fd707da").SendAsync("ReceiveMessage", request);

            return Ok("done");
        }
    }
}