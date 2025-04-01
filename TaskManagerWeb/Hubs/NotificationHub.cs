using Microsoft.AspNetCore.SignalR;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Entities;
using TaskManagerWeb.Models;

namespace TaskManagerWeb.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly AppDbContext _db;
        public NotificationHub(AppDbContext db)
        {
            _db = db;
        }

        private static Dictionary<string, List<string>> NtoIdMappingTable = new Dictionary<string, List<string>>();
        public override async Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            var userId = Context.UserIdentifier;
            List<string> userIds;

            //store the userid to the list.  
            if (!NtoIdMappingTable.TryGetValue(username, out userIds))
            {
                userIds = new List<string>();
                userIds.Add(userId);

                NtoIdMappingTable.Add(username, userIds);
            }
            else
            {
                userIds.Add(userId);
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.Identity.Name;
            var userId = Context.UserIdentifier;
            List<string> userIds;

            //remove userid from the List  
            if (NtoIdMappingTable.TryGetValue(username, out userIds))
            {
                userIds.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        //public async Task SendNotification(string message)
        //{
        //    await Clients.All.SendAsync("SendNotification", message);
        //}
    }
}
