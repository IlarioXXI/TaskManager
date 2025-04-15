using FluentValidation;
using TaskManager.Models;
using TaskManagerWeb.Api.ViewModels.UserViewModels;
using TaskManagerWEB.Api.ViewModels.UserViewModels;

namespace TaskManagerWEB.Api.Validators.UserValidators
{
    public class ChangePasswordValidation : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidation()
        {
            RuleFor(p => p.NewPassword).Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.");
            RuleFor(p => p.NewPassword).Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            RuleFor(p => p.NewPassword).Matches(@"[\!\?\*\.]*$").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(p => p.ConfirmNewPassword).Equal(p => p.NewPassword);
        }
    }
}
