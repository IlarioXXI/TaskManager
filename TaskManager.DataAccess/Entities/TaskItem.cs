using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskManager.DataAccess.Utility;

namespace TaskManager.DataAccess.Entities
{
    public class TaskItem
    {



        [Key]
        public int Id { get; set; }

        public int TeamId { get; set; }
        [ForeignKey("TeamId")]
        [ValidateNever]
        public Team? Team { get; set; }
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        [ValidateNever]
        public Status? Status { get; set; }



        public int PriorityId { get; set; }
        [ForeignKey("PriorityId")]
        [ValidateNever]
        public Priority? Priority { get; set; }




        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Comment>? Comments { get; set; }

        [JsonIgnore]
        public List<History>? History { get; set; }
        public DateTime? DueDate { get; set; }
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public string? AppUserId { get; set; }

        public DateTime TaskNotification { get; set; }
    }
}
