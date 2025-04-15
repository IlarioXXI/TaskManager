using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace TaskManagerWeb.Api.ViewModels.UserViewModels
{
    public class RegisterModel
    {
        public  string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
}
