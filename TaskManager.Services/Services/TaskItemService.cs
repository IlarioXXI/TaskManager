using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserClaimService _userClaimService;
        private readonly ILogger<TaskItemService> _logger;

        public TaskItemService(IUnitOfWork unitOfWork, IUserClaimService userClaimService,ILogger<TaskItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _userClaimService = userClaimService;
            _logger = logger;   
        }
        public bool Delete(int id)
        {
            var userId = _userClaimService.GetUserId();
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            var role = _userClaimService.GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != SD.Role_Admin)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this task.");
            }
            if (_unitOfWork.TaskItem.Get(x=>x.Id == id) == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }
            _unitOfWork.TaskItem.Remove(_unitOfWork.TaskItem.Get(x => x.Id == id));
            _unitOfWork.Save();
            _logger.LogInformation("TaskItem ({taskItemId}) deleted successfully by : {email}", id, user.Email);
            return true;
        }

        public IEnumerable<TaskItem> GetAll()
        {
            var userId = _userClaimService.GetUserId();
            var taskItems = _unitOfWork.TaskItem.GetAll(t => t.AppUserId == userId, includeProperties: "Status,Priority,Comments,Team");
            return taskItems;
        }

        public TaskItem GetById(int id)
        {
            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == id,includeProperties:"Status,Priority,Comments,Team");
            return taskItem;
        }

        public TaskItem Upsert(TaskItem taskItem)
        {
            var userId = _userClaimService.GetUserId();                                 
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);

            if (taskItem.Id == 0)
            {
                _unitOfWork.TaskItem.Add(taskItem);
                _unitOfWork.Save();
                _logger.LogInformation("TaskItem ({taskItemName}) created successfully by : {email}", taskItem.Title, user.Email);
                return taskItem;
            }
            else
            {
                var existingItem = _unitOfWork.TaskItem.Get(t => t.Id == taskItem.Id);
                var oldStatus = _unitOfWork.Status.Get(s => s.Id == existingItem.StatusId);
                var oldPriority = _unitOfWork.Priority.Get(p => p.Id == existingItem.PriorityId);
                existingItem.StatusId = taskItem.StatusId;
                existingItem.PriorityId = taskItem.PriorityId;
                if (oldStatus.Id != existingItem.StatusId || oldPriority.Id != existingItem.PriorityId)
                {
                    History history = new()
                    {
                        FromStatus = oldStatus.Name,
                        ToStatus = _unitOfWork.Status.Get(s => s.Id == existingItem.StatusId).Name,
                        ChangeDate = DateTime.Now,
                        TaskItemId = existingItem.Id,
                        AppUserId = userId,
                    };
                    _unitOfWork.History.Add(history);
                    _unitOfWork.Save();
                }

                _unitOfWork.TaskItem.Update(taskItem);
                taskItem.Status = oldStatus;
                taskItem.Priority = oldPriority;
                _unitOfWork.Save();
                _logger.LogInformation("TaskItem ({taskItemName}) updated successfully by : {email}", taskItem.Title, user.Email);
                return taskItem;
            }
        }
    }
}
