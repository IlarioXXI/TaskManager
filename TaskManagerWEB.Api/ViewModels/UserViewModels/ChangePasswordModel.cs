using FluentValidation;

namespace TaskManagerWeb.Api.ViewModels.UserViewModels
{
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    
}
