using FluentValidation;

namespace TaskManagerWEB.Api.models
{
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

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
