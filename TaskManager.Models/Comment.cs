using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey("TaskItemId")]
        public TaskItem? TaskItem { get; set; }

        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; }

    }

    
}
