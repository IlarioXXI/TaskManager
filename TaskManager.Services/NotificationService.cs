
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TaskManager.DataAccess;
using TaskManager.Models;
using TaskManager.Services.Hubs;


namespace TaskManager.Services
{
    public class NotificationService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;


        public NotificationService(IHubContext<NotificationHub> hubContext,
            IServiceScopeFactory scopeFactory)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await Task.Delay(10000);

            while (!stoppingToken.IsCancellationRequested)
            {
                // potrei inserire lo scope in un using per liberare le risorse e non avre problemi di memory leak
                var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var users = db.Users.ToList();

                if (users != null)
                {
                    foreach (var u in users)
                    {
                        List<TaskItem>? taskItems = new List<TaskItem>();
                        if (db.TaskItems.All(x => x.AppUserId == u.Id) != null)
                        {
                            foreach (var item in db.TaskItems.Where(x => x.AppUserId == u.Id))
                            {
                                taskItems.Add(item);
                            }
                        }
                        var taskItemsToNotify = new List<TaskItem>();
                        foreach (var i in taskItems)
                        {
                            //lo posso concatenare l'if ma penso sia piu semplice da leggere
                            if (db.Status.FirstOrDefault(s => s.Id == i.StatusId).Name != "completed" && NotificationHub.IsUserConnected(u.Id))
                            {
                                if (i.DueDate.Value <= DateTime.Now.AddDays(1))
                                {
                                    if (i.TaskNotification.AddDays(1) <= DateTime.Now)
                                    {
                                        db.TaskItems.FirstOrDefault(t => t.Id == i.Id).TaskNotification = DateTime.Now;
                                        db.TaskItems.Update(i);
                                        db.SaveChanges();
                                        taskItemsToNotify.Add(i);
                                    }
                                }
                            }

                        }


                        if (!taskItemsToNotify.IsNullOrEmpty() && NotificationHub.IsUserConnected(u.Id))
                        {
                            await Task.Delay(20000);
                            await _hubContext.Clients.User(u.Id).SendAsync("SendNotification", JsonConvert.SerializeObject(taskItemsToNotify));
                        }
                    }

                }


                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
