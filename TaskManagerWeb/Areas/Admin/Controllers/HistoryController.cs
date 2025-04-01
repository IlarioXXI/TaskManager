using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using TaskManager.DataAccess.Repositories.Interfaces;
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
            var taskFromTb = _unitOfWork.TaskItem.Get(t=>t.Id == id,includeProperties:"History,Status,Priority");
            var appUserIdHistory = _unitOfWork.History.Get(h => h.TaskItemId == id).AppUserId;
            var taskItemVM = new ToDoVM()
            {
                TaskToDo = taskFromTb,
                AppUser = _unitOfWork.AppUser.Get(u=>u.Id == appUserIdHistory),
            };
            foreach (var user in taskItemVM.TaskToDo.History)
            {
                user.AppUser = _unitOfWork.AppUser.Get(u=>u.Id == user.AppUserId);
            }
            return View(taskItemVM);
        }
    }
}
