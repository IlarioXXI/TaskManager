using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace TaskManagerWEB.Api.ViewModels.UserViewModels
{
    public class RegisterModel
    {
        public  string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
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
