using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManagerWeb.Models;

namespace TaskManagerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaskItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TaskItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Upsert(int? id, int teamId)
        {
            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == id);
            var teamFromDb = _unitOfWork.Team.Get(t=>t.Id == teamId);
            var userList = new List<AppUser>();


            var users = _unitOfWork.AppUser.GetAll(u=>u.Teams.Contains(teamFromDb));

            foreach (var u in users)
            {
                userList.Add(u);
            }
            if (id==0 || id==null)
            {

                ToDoVM taskItemVM = new()
                {
                    TaskToDo = new TaskItem(),
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
                    Users = userList.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    
                };
                
                taskItemVM.TaskToDo.TeamId = teamId;
                return View(taskItemVM);
            }
            else
            {

                
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
                    Users = userList.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),


                };






                //if (id != 0)
                //{
                //    task.TaskToDo = _unitOfWork.TaskItem.Get(t => t.Id == id, includeProperties: "History,Comments");
                //    if (task.TaskToDo.Status == null)
                //    {
                //        task.TaskToDo.Status = _unitOfWork.Status.Get(s => s.Id == task.TaskToDo.StatusId);
                //    }
                //    if (task.TaskToDo.Priority == null)
                //    {
                //        task.TaskToDo.Priority = _unitOfWork.Priority.Get(s => s.Id == task.TaskToDo.PriorityId);
                //    }
                //    task.PrioritySelectedId = task.TaskToDo.PriorityId;
                //    task.StatusSelectedId = task.TaskToDo.StatusId;
                //}
                
                    return View(task);

            }

        }

        [HttpPost]
        public IActionResult Upsert(ToDoVM taskItemVM)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //taskItemVM.TaskToDo.AppUserId = claim;

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
                    var oldStatusId = _unitOfWork.TaskItem.Get(t => t.Id == taskItemVM.TaskToDo.Id).StatusId;
                    if (oldStatusId != taskItemVM.StatusSelectedId)
                    {
                        History history = new()
                        {
                            FromStatus = _unitOfWork.Status.Get(s => s.Id == oldStatusId).Name,
                            ToStatus = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId).Name,
                            ChangeDate = DateTime.Now,
                            TaskItemId = taskItemVM.TaskToDo.Id,
                            AppUserId = _unitOfWork.TaskItem.Get(t => t.Id == taskItemVM.TaskToDo.Id).AppUserId
                        };
                        _unitOfWork.History.Add(history);
                    }
                    //var statusId = _unitOfWork.TaskItem.Get(t=>t.Id == taskItemVM.TaskToDo.Id).StatusId;
                    //var priorityId = _unitOfWork.TaskItem.Get(t => t.Id == taskItemVM.TaskToDo.Id).PriorityId;

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
    }
}
