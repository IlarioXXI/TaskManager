using FluentValidation;
using TaskManagerWeb.Api.ViewModels;
using TaskManagerWEB.Api.ViewModels;

namespace TaskManagerWEB.Api.Validators
{
    public class TeamValidation : AbstractValidator<TeamVM>
    {
        public TeamValidation()
        {
            RuleFor(x => x.Users).Null();
            RuleFor(x => x.TaskItems).Null();
            RuleFor(x => x.UsersIds).NotNull();
            RuleFor(x => x.taskItemsIds).NotNull();
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must be at least 3 characters long");
        }
    }
}
