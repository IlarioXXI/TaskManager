using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;

namespace TaskManagerWeb.Models
{
    public class ToDoVM
    {
        public TaskItem? TaskToDo { get; set; }
        public AppUser? AppUser { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> PriorityList { get; set; }
        public int PrioritySelectedId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public int StatusSelectedId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Users { get; set; }
    }

    

    
}
