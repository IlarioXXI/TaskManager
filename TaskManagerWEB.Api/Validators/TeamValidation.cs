using FluentValidation;
using TaskManagerWeb.Api.ViewModels;

namespace TaskManagerWEB.Api.Validators
{
    public class TeamValidation : AbstractValidator<TeamVM>
    {
        public TeamValidation()
        {
            RuleFor(x => x.Users).Null();
            RuleFor(x => x.TaskItems).Null();
            RuleFor(x => x.SelectedUserIds).NotNull();
            RuleFor(x => x.Team).NotNull();
        }
    }
}
