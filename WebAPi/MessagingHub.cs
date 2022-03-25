using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Shared.Commands;

namespace WebAPi.Services;

public static class UserHandler
{
    public static List<MessageCommand> Connections = new()
    {
        //new MessageCommand()
        //{
        //    BrandId = 1,
        //    BranchId = 1
        //},
        //new MessageCommand()
        //{
        //    BrandId = 1,
        //    BranchId = 2
        //},
        //new MessageCommand()
        //{
        //    BrandId = 2,
        //    BranchId = 1
        //},
        //new MessageCommand()
        //{
        //    BrandId = 2,
        //    BranchId = 1
        //},
        //new MessageCommand()
        //{
        //    BrandId = 2,
        //    BranchId = 3
        //},

    };
    public static int InitStatusId = 1;
}


public class MessagingHub : Hub
{

    public const string HubUrl = "/MessagingHub";

    public override Task OnConnectedAsync()
    {
        MessageCommand obj = UserHandler.Connections.FirstOrDefault(e => e.connId == Context.ConnectionId);
        if (obj == null)
        {
            MessageCommand obj2 = new MessageCommand()
            {
                connId = Context.ConnectionId,
                StatusId = UserHandler.InitStatusId,
                //BrandId = Convert.ToInt32 (Context.Items.FirstOrDefault(e=>e.Key == "BrandId").Value),
                //BranchId = Convert.ToInt32(Context.Items.FirstOrDefault(e => e.Key == "BranchId").Value),
                BrandId = 1,
                BranchId = 2,
            };
            UserHandler.Connections.Add(obj2);
            Console.WriteLine("cont: " + Context.ConnectionId);
        }


        return base.OnConnectedAsync();
    }


    public override async Task OnDisconnectedAsync(Exception e)
    {
        MessageCommand obj = new MessageCommand()
        {
            connId = Context.ConnectionId,
        };

        UserHandler.Connections.Remove(obj);
        await base.OnDisconnectedAsync(e);
    }

}