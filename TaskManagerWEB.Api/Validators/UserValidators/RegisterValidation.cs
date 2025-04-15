using FluentValidation;
using TaskManager.Models;
using TaskManagerWeb.Api.ViewModels.UserViewModels;
using TaskManagerWEB.Api.ViewModels.UserViewModels;

namespace TaskManagerWEB.Api.Validators.UserValidators
{
    public class RegisterValidation : AbstractValidator<RegisterModel>
    {
        public RegisterValidation()
        {
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.Role).NotNull();
            RuleFor(p => p.Password).Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.");
            RuleFor(p => p.Password).Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            RuleFor(x => x.Password).Matches(@"[\!\?\*\.]*$").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Your passwords don't match.");
            //RuleFor(x => x.Password).Must(MyPasswordValidator);
        }

    }
}
