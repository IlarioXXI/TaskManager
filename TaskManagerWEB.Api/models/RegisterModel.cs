using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace TaskManagerWEB.Api.models
{
    public class RegisterModel
    {
        public  string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }

    public class RegisterValidation : AbstractValidator<RegisterModel>
    {
        public RegisterValidation()
        {
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.Role).NotNull();
            RuleFor(p => p.Password).Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.");
            RuleFor(p => p.Password).Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            RuleFor(x => x.Password).Matches(@"[\!\?\*\.]*$").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(x=>x.ConfirmPassword).Equal(x=>x.Password).WithMessage("Your passwords don't match.");
            //RuleFor(x => x.Password).Must(MyPasswordValidator);
        }

    }


    //esempio per validazione customizzata
    //public static class MyCustomValidators
    //{
    //    public static IRuleBuilderOptions<T, IList<TElement>> MyPasswordValidator<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, 5)
    //    {
    //        return ruleBuilder.Must(password => password.Count < 5).WithMessage("The password must have at least one uppercase letter, at least one special character, at least one number.");
    //    }
    //}
}
