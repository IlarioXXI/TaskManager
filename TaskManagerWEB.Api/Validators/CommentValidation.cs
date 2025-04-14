using FluentValidation;
using TaskManagerWeb.Api.Models;

namespace TaskManagerWEB.Api.Validators
{
    public class CommentValidation : AbstractValidator<CommentVM>
    {
        public CommentValidation()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.CreationDate).Empty();
            RuleFor(x => x.TaskItemId).NotNull();
            RuleFor(x => x.TaskItemTitle).Null();
            RuleFor(x => x.AppUserId).Null();
            RuleFor(x => x.AppUserEmail).Null();
        }
    }
}
