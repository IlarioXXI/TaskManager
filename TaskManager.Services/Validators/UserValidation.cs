using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services.Validators
{
    public class UserValidation : AbstractValidator<AppUser>
    {
        public UserValidation()
        {
            RuleFor(x => x.Email).EmailAddress().NotNull();
            RuleFor(x => x.UserName).NotNull();
            RuleFor(x => x.EmailConfirmed).Null();
            RuleFor(x => x.PasswordHash).Null();
            RuleFor(x => x.SecurityStamp).Null();
            RuleFor(x => x.PhoneNumber).Null();
            RuleFor(x => x.PhoneNumberConfirmed).Null();
            RuleFor(x => x.TwoFactorEnabled).Null();
            RuleFor(x => x.LockoutEnabled).Null();
            RuleFor(x => x.AccessFailedCount).Null();
            RuleFor(x => x.Name).Null();
            RuleFor(x => x.Role).Null();
        }
    }
}
