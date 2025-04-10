using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManagerWeb.Models;

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
        public IActionResult Upsert(int? id, int teamId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == id);
            ToDoVM task = new()
            {
                TaskToDo = taskItem,
                PriorityList = _unitOfWork.Priority.GetAll().Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }),
                StatusList = _unitOfWork.Status.GetAll().Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }),
                Users = _unitOfWork.AppUser.GetAll(u => u.Id == taskItem.AppUserId).Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })

            };

            task.TaskToDo.Team = _unitOfWork.Team.Get(t => t.Id == teamId);

            if (id == 0 || id == null)
            {
                task.TaskToDo = new TaskItem();
                task.TaskToDo.TeamId = teamId;
                return View(task);
            }
            else
            {

                task.TaskToDo = _unitOfWork.TaskItem.Get(t => t.Id == id, includeProperties: "History,Comments");
                if (task.TaskToDo.Status == null)
                {
                    task.TaskToDo.Status = _unitOfWork.Status.Get(s => s.Id == task.TaskToDo.StatusId);
                }
                if (task.TaskToDo.Priority == null)
                {
                    task.TaskToDo.Priority = _unitOfWork.Priority.Get(s => s.Id == task.TaskToDo.PriorityId);
                }
                task.PrioritySelectedId = task.TaskToDo.PriorityId;
                task.StatusSelectedId = task.TaskToDo.StatusId;


                task.TaskToDo.Team = _unitOfWork.Team.Get(t => t.Id == teamId, includeProperties: "Users");
                
                if (userId.IsNullOrEmpty())
                {
                    task.AppUser = _unitOfWork.AppUser.Get(u => u.Id == userId);
                }
                return View(task);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ToDoVM taskItemVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            if (ModelState.IsValid)
            {
                if (taskItemVM.TaskToDo.Id == 0)
                {

                    taskItemVM.TaskToDo.StatusId = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId).Id;
                    taskItemVM.TaskToDo.PriorityId = _unitOfWork.Priority.Get(p => p.Id == taskItemVM.PrioritySelectedId).Id;
                    _unitOfWork.TaskItem.Add(taskItemVM.TaskToDo);
                }
                else
                {
                    var userIdByDb = _unitOfWork.TaskItem.Get(t=>t.Id == taskItemVM.TaskToDo.Id).AppUserId;
                    if (userIdByDb == userId)
                    {
                        var oldStatusId = _unitOfWork.TaskItem.Get(t => t.Id == taskItemVM.TaskToDo.Id).StatusId;
                        if (oldStatusId != taskItemVM.StatusSelectedId)
                        {
                            History history = new()
                            {
                                FromStatus = _unitOfWork.Status.Get(s => s.Id == oldStatusId).Name,
                                ToStatus = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId).Name,
                                ChangeDate = DateTime.Now,
                                TaskItemId = taskItemVM.TaskToDo.Id,
                                AppUserId = userIdByDb
                            };
                            _unitOfWork.History.Add(history);
                        }
                    }
                    else
                    {
                        var oldTaskItem = _unitOfWork.TaskItem.Get(t => t.Id == taskItemVM.TaskToDo.Id);
                        taskItemVM.TaskToDo.StatusId = oldTaskItem.StatusId;
                        taskItemVM.TaskToDo.PriorityId = oldTaskItem.PriorityId;
                        taskItemVM.TaskToDo.AppUserId = oldTaskItem.AppUserId;
                    }
                        
         

                    taskItemVM.TaskToDo.Status = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId);
                    taskItemVM.TaskToDo.Priority = _unitOfWork.Priority.Get(p => p.Id == taskItemVM.PrioritySelectedId);


                    _unitOfWork.TaskItem.Update(taskItemVM.TaskToDo);
                }

                _unitOfWork.Save();
                taskItemVM.TaskToDo.Status = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId);
                taskItemVM.TaskToDo.Priority = _unitOfWork.Priority.Get(s => s.Id == taskItemVM.PrioritySelectedId);
                return RedirectToAction("Index", "Team");
            }
            return View(taskItemVM);
        }

        public IActionResult Details(int id)
        {
            var task = _unitOfWork.TaskItem.Get(u => u.Id == id, includeProperties: "Priority,Status,Comments");
            var comments = new List<Comment>();
            foreach (var c in _unitOfWork.Comment.GetAll(c => c.TaskItemId == id, includeProperties: "AppUser").ToList())
            {
                c.AppUser = _unitOfWork.AppUser.Get(u => u.Id == c.AppUserId);
                comments.Add(c);
            }
            task.Comments = comments;
            return View(task);
        }


        public IActionResult Delete(int id)
        {
            var taskToDelete = _unitOfWork.TaskItem.Get(t => t.Id == id);
            _unitOfWork.TaskItem.Remove(taskToDelete);
            _unitOfWork.Save();
            return RedirectToAction("Index", "Home");
        }

    }
}
