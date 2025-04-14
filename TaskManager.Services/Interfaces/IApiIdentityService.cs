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
        Task<string> CreateJwtTokenAsync(AuthUser authUser);
        Task<string> RegisterAsync(RegisterModel model);
        IEnumerable<AppUser> GetAllUsers();
        Task<bool> MyChangePasswordAsync(ChangePasswordModel model);
    }
}
