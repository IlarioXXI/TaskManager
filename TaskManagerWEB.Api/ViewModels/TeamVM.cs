using TaskManager.Models;

namespace TaskManagerWEB.Api.ViewModels
{
    public class TeamVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<TaskItem>? TaskItems { get; set; }
        public List<int> taskItemsIds { get; set; }

        public List<string> UsersIds { get; set; }
        public List<AppUser> Users { get; set; }
    }
}
