using FluentValidation;
using TaskManagerWeb.Api.ViewModels;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.ViewModels;


namespace TaskManagerWEB.Api.Validators
{
    public class TaskItemValidation : AbstractValidator<TaskItemVM>
    {
        public TaskItemValidation()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.StatusId).NotNull();
            RuleFor(x => x.PriorityId).NotNull();
            RuleFor(x => x.Description).Length(1, 500).NotNull();
            RuleFor(x => x.TeamId).NotNull();
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .Length(1, 100)
                .WithMessage("Title must be between 1 and 100 characters long.");
            RuleFor(x => x.TeamName).Null();
            RuleFor(x=>x.TeamId).NotNull()
                .WithMessage("TeamId is required.");
        }
    }
}
