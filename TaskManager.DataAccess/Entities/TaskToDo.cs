using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Entities
{
    public class TaskToDo
    {

        public enum StatusEnum
        {
            ToDo,
            InProgress,
            Done
        }
        public enum PriorityEnum
        {
            Low,
            Medium,
            High
        }


        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Comment> Comments { get; set; }
        public string History { get; set; }
        public DateTime DueDate { get; set; }
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public string AppUserId { get; set; }
    }
}
