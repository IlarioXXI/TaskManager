﻿using FluentValidation;

namespace TaskManagerWEB.Api.models
{
    public class AuthUser
    {
        //public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


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
