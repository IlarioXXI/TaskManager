using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models;
using TaskManagerWeb.Api.Models;

namespace TaskManagerWEB.Api.ViewModels
{
    public class TaskItemVM
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; } 
        public int PriorityId { get; set; }
        public string? PriorityName { get; set; } 
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public List<CommentVM>? Comments { get; set; } 
        public DateTime? DueDate { get; set; }
        public string? AppUserId { get; set; }
    }
}
