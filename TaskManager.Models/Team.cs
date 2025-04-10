using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<TaskItem>? TaskItems { get; set; }


        public List<AppUser> Users { get; set; } = [];
    }
}
