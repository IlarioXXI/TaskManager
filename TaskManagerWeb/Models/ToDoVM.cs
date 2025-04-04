using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.DataAccess.Entities;

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

    

    public class TaskItemValidation : AbstractValidator<ToDoVM>
    {
        public TaskItemValidation()
        {
            RuleFor(x=>x.TaskToDo).NotNull();
            RuleFor(x => x.TaskToDo.AppUserId).NotNull();
            RuleFor(x => x.TaskToDo.StatusId).NotNull();
            RuleFor(x => x.TaskToDo.PriorityId).NotNull();
            RuleFor(x => x.TaskToDo.Description).NotNull();
            RuleFor(x => x.TaskToDo.DueDate).NotNull();

            RuleFor(x => x.TaskToDo.History).Null();
            RuleFor(x => x.TaskToDo.Status).Null();
            RuleFor(x => x.TaskToDo.Priority).Null();
            RuleFor(x => x.TaskToDo.Comments).Null();
            RuleFor(x=>x.AppUser).Null();
            RuleFor(x=> x.PriorityList).Null();
            RuleFor(x=>x.Users).Null();
            RuleFor(x => x.StatusList).Null();
        }
    }
}
