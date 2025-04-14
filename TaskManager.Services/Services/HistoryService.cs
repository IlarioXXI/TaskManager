using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUnitOfWork _unitOfWork;

        public HistoryService(IHttpContextAccessor httpContext,IUnitOfWork unitOfWork)
        {
            _httpContext = httpContext; 
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<History> GetAllByTaskId(int taskId)
        {
            if (!_httpContext.HttpContext.User.IsInRole(SD.Role_Admin))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                var historyTask = _unitOfWork.History.GetAll(h => h.TaskItemId == taskId);
                return historyTask;
            }
        }
    }
}
