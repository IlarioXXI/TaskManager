using Microsoft.AspNetCore.SignalR;
using TaskManager.DataAccess;

namespace TaskManager.Services.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly AppDbContext _db;
        public NotificationHub(AppDbContext db)
        {
            _db = db;
        }

        private static Dictionary<string, string> _connectedUsers = new Dictionary<string, string>();
        public override async Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            var userId = Context.UserIdentifier;

            //store the userid to the list.  
            if (!_connectedUsers.TryGetValue(userId, out username))
            {
                _connectedUsers.Add(userId, Context.User.Identity.Name);
            }
          

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.Identity.Name;
            var userId = Context.UserIdentifier;

            //remove userid from the List  
            if (_connectedUsers.TryGetValue(userId, out username))
            {
                _connectedUsers.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public static bool IsUserConnected(string userId)
        {

            return _connectedUsers.ContainsKey(userId);
        }

    }
}
