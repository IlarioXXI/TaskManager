using FluentValidation;

namespace TaskManager.Models
{
    public class AuthUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }


    
}
