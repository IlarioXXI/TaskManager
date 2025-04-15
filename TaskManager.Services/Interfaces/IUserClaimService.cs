using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services.Interfaces
{
    public interface IUserClaimService
    {
        AppUser GetUserTracked();
        string GetUserId();
        string GetUSerEmail();
        IEnumerable<Claim> GetClaims();
        AppUser GetUser();
    }
}
