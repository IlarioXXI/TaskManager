using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using TaskManager.DataAccess.Interfaces;
using TaskManagerWeb.Models;

namespace TaskManagerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int id)
        {
            var history = _unitOfWork.History.Get(h=>h.TaskItemId == id, includeProperties:"TaskItem");
            if (history == null)
            {
                return View("NotFound");
            }
            history.TaskItem = _unitOfWork.TaskItem.Get(t => t.Id == history.TaskItemId, includeProperties: "Priority,Status,Team,History");
            if (_unitOfWork.History.Get(h => h.TaskItemId == id) != null) 
            {
                var appUserIdHistory = _unitOfWork.History.Get(h => h.TaskItemId == id).AppUserId;
                var taskItemVM = new ToDoVM()
                {
                    TaskToDo = history.TaskItem,
                    AppUser = _unitOfWork.AppUser.Get(u => u.Id == appUserIdHistory),
                };
                foreach (var user in taskItemVM.TaskToDo.History)
                {
                    user.AppUser = _unitOfWork.AppUser.Get(u => u.Id == user.AppUserId);
                }
                return View(taskItemVM);
            }
            var newTaskItem = new ToDoVM()
            {
                TaskToDo = history.TaskItem,
                AppUser = null,
            };
            return View(newTaskItem);


        }
    }
}
