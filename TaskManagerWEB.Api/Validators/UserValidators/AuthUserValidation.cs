using FluentValidation;
using TaskManager.Models;
using TaskManagerWEB.Api.ViewModels.UserViewModels;

namespace TaskManagerWEB.Api.Validators.UserValidators
{
    public class AuthUserValidation : AbstractValidator<AuthUser>
    {
        public AuthUserValidation()
        {
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(p => p.Password).Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.");
            RuleFor(p => p.Password).Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            RuleFor(x => x.Password).Matches(@"[\!\?\*\.]*$").WithMessage("Your password must contain at least one (!? *.).");
        }
    }
}
