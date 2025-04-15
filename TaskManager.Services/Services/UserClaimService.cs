using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Services
{
    public class UserClaimService : IUserClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public UserClaimService(IHttpContextAccessor httpContextAccessor,IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<Claim> GetClaims()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims;
        }

        public AppUser GetUser()
        {
            var userId = GetUserId();
            return _unitOfWork.AppUser.Get(u=>u.Id == userId);
        }
        public AppUser GetUserTracked()
        {
            var userId = GetUserId();   
            return _unitOfWork.AppUser.Get(u => u.Id == userId,tracked:true);
        }
        public string GetUSerEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            return userId;
        }
    }
}
