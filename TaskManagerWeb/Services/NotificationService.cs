
using Microsoft.AspNetCore.SignalR;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Entities;
using TaskManagerWeb.Hubs;


namespace askManagerWeb.Services
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
            while (!stoppingToken.IsCancellationRequested)
            {
                    // potrei inserire lo scope in un using per liberare le risorse e non avre problemi di memory leak
                    var scope = _scopeFactory.CreateScope();
                
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var users = db.Users.ToList();

                    if (users!=null) 
                    {
                        foreach (var u in users)
                        {
                            List<TaskItem>? taskItems = new List<TaskItem>();
                            if (db.TaskItems.FirstOrDefault(x => x.AppUserId == u.Id)!=null)
                            {
                                taskItems.Add(db.TaskItems.FirstOrDefault(x => x.AppUserId == u.Id));
                            }
                            foreach (var i in taskItems)
                            {
                                if (i.DueDate <= DateTime.Now)
                                {
                                    //tmuovere dalla lista l'attività scaduto o mettere un soft delete
                                    //aggiungedno il soft potremmo escludere la possibilità di inviare in loop la notifica di un'attività
                                    var message = $"La tua attività {i.Id} è scaduta";
                                    await _hubContext.Clients.User(u.Id).SendAsync("SendNotification", message);
                                    Console.WriteLine(u.Id);
                                }
                            }
                        }
                    }
                    
                
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
