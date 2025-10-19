using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManagerWeb.Models;
using System.Collections.Generic;

namespace TaskManagerWeb.Areas.User.Controllers
{
    [Area("User")]
    public class TaskItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /User/TaskItem/Upsert?id=...&teamId=...
        public IActionResult Upsert(int? id, int teamId)
        {
            if (id == null || id == 0)
            {
                return BadRequest();
            }

            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == id, includeProperties: "Priority,Status,Team,History");
            if (taskItem == null) return View("NotFound");

            // user can only change status; but we still pass Users so UI can show assignment (disabled)
            var teamFromDb = _unitOfWork.Team.Get(t => t.Id == teamId);
            var users = _unitOfWork.AppUser.GetAll(u => u.Teams.Contains(teamFromDb)).ToList();

            var vm = new ToDoVM()
            {
                TaskToDo = taskItem,
                PriorityList = _unitOfWork.Priority.GetAll().Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }),
                StatusList = _unitOfWork.Status.GetAll().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }),
                Users = users.Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() }),
                SelectedUserId = taskItem.AppUserId,
                PrioritySelectedId = taskItem.PriorityId,
                StatusSelectedId = taskItem.StatusId
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Upsert(ToDoVM vm)
        {
            if (vm == null || vm.TaskToDo == null) return BadRequest();

            // Only update Status for user area. Keep other fields intact.
            var dbTask = _unitOfWork.TaskItem.Get(t => t.Id == vm.TaskToDo.Id);
            if (dbTask == null) return View("NotFound");

            var oldStatusId = dbTask.StatusId;
            if (oldStatusId != vm.StatusSelectedId)
            {
                // add history record
                History history = new()
                {
                    FromStatus = _unitOfWork.Status.Get(s => s.Id == oldStatusId).Name,
                    ToStatus = _unitOfWork.Status.Get(s => s.Id == vm.StatusSelectedId).Name,
                    ChangeDate = DateTime.Now,
                    TaskItemId = dbTask.Id,
                    AppUserId = dbTask.AppUserId
                };
                _unitOfWork.History.Add(history);

                dbTask.StatusId = vm.StatusSelectedId;
                dbTask.Status = _unitOfWork.Status.Get(s => s.Id == vm.StatusSelectedId);
                _unitOfWork.TaskItem.Update(dbTask);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index", "Team");
        }

        public IActionResult Details(int id)
        {
            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == id, includeProperties: "Priority,Status,Team,Comments,History");
            if (taskItem == null)
            {
                return View("NotFound");
            }
            else
            {
                foreach (var c in taskItem.Comments)
                {
                    c.AppUser = _unitOfWork.AppUser.Get(u => u.Id == taskItem.AppUserId);
                }
            }
            return View(taskItem);
        }
    }
}
