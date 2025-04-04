using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TaskManager.DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        [AllowNull]
        public string? Name { get; set; }


        [JsonIgnore]
        public List<Team> Teams { get; set; } = [];

        [JsonIgnore]
        [NotMapped]
        [AllowNull]
        public string? Role { get; set; }
    }

    public class UserValidation : AbstractValidator<AppUser>
    {
        public UserValidation()
        {
            RuleFor(x=>x.Email).EmailAddress().NotNull();
            RuleFor(x=>x.UserName).NotNull();
            RuleFor(x=>x.NormalizedEmail).Null();
            RuleFor(x => x.NormalizedUserName).Null();
            RuleFor(x => x.EmailConfirmed).Null() ;
            RuleFor(x => x.PasswordHash).Null();
            RuleFor(x => x.SecurityStamp).Null();
            RuleFor(x=>x.PhoneNumber).Null();
            RuleFor(x=>x.PhoneNumberConfirmed).Null();
            RuleFor(x=>x.TwoFactorEnabled).Null();
            RuleFor(x=>x.LockoutEnd).Null();
            RuleFor(x=>x.LockoutEnabled).Null();
            RuleFor(x => x.AccessFailedCount).Null();
            RuleFor(x => x.Name).Null();
            RuleFor(x => x.Role).Null();
        }
    }
}
