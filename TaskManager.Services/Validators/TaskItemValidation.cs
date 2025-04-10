using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerWeb.Models;

namespace TaskManager.Services.Validators
{
    public class TaskItemValidation : AbstractValidator<ToDoVM>
    {
        public TaskItemValidation()
        {
            RuleFor(x => x.TaskToDo).NotNull();
            RuleFor(x => x.TaskToDo.AppUserId).NotNull();
            RuleFor(x => x.TaskToDo.StatusId).NotNull();
            RuleFor(x => x.TaskToDo.PriorityId).NotNull();
            RuleFor(x => x.TaskToDo.Description).NotNull();
            RuleFor(x => x.TaskToDo.DueDate).NotNull();

            RuleFor(x => x.TaskToDo.History).Null();
            RuleFor(x => x.TaskToDo.Status).Null();
            RuleFor(x => x.TaskToDo.Priority).Null();
            RuleFor(x => x.TaskToDo.Comments).Null();
            RuleFor(x => x.AppUser).Null();
            RuleFor(x => x.PriorityList).Null();
            RuleFor(x => x.Users).Null();
            RuleFor(x => x.StatusList).Null();
        }
    }
}
