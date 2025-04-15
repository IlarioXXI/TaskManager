using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services.Interfaces
{
    public interface IApiIdentityService
    {
        Task<string> CreateJwtTokenAsync(string email, string pass);
        Task<string> RegisterAsync(string email, string pass, string roleTOAdd);
        IEnumerable<AppUser> GetAllUsers();
        Task<bool> MyChangePasswordAsync(string currentPass, string newPass);
    }
}
