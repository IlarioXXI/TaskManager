using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.DataAccess;
using TaskManager.Models;
using TaskManager.Services.Hubs;

namespace TaskManager.Services.Services
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
            // iniziale delay
            await Task.Delay(10000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var users = db.Users.ToList();

                if (users != null && users.Any())
                {
                    foreach (var u in users)
                    {
                        var taskItems = db.TaskItems
                            .Where(x => x.AppUserId == u.Id)
                            .ToList();

                        var taskItemsToNotify = new List<TaskItem>();
                        var userConnected = NotificationHub.IsUserConnected(u.Id);

                        foreach (var i in taskItems)
                        {
                            // recupera lo status in modo sicuro
                            var status = db.Status.FirstOrDefault(s => s.Id == i.StatusId);
                            if (status == null) continue;

                            // salta se completed o se l'utente non è connesso
                            if (string.Equals(status.Name, "completed", StringComparison.OrdinalIgnoreCase) || !userConnected)
                                continue;

                            // controlla che DueDate sia valorizzata
                            if (!i.DueDate.HasValue)
                                continue;

                            if (i.DueDate.Value <= DateTime.Now.AddDays(1))
                            {
                                // TASK: TaskNotification è non-nullable (DateTime). Usa default(DateTime) come "mai notificato".
                                if (i.TaskNotification == default(DateTime) || i.TaskNotification.AddDays(1) <= DateTime.Now)
                                {
                                    i.TaskNotification = DateTime.Now;
                                    db.TaskItems.Update(i);
                                    db.SaveChanges();
                                    taskItemsToNotify.Add(i);
                                }
                            }
                        }

                        if (taskItemsToNotify.Any() && userConnected)
                        {
                            await Task.Delay(20000, stoppingToken);
                            await _hubContext.Clients.User(u.Id).SendAsync("SendNotification", JsonConvert.SerializeObject(taskItemsToNotify), stoppingToken);
                        }
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
