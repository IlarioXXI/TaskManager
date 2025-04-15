using FluentValidation;
using TaskManagerWeb.Api.ViewModels;
using TaskManagerWEB.Api.ViewModels;

namespace TaskManagerWEB.Api.Validators
{
    public class TeamValidation : AbstractValidator<TeamVM>
    {
        public TeamValidation()
        {
            RuleFor(x => x.Users).Empty();
            RuleFor(x => x.TaskItems).Null();
            RuleFor(x => x.UsersIds).NotNull();
            RuleFor(x => x.taskItemsIds).NotNull();
            RuleFor(x => x.Name).NotNull().WithMessage("Name is required");
        }
    }
}
