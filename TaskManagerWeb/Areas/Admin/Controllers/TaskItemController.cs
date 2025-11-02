using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using TaskManager.DataAccess.Interfaces;
using TaskManagerWeb.Models;
using System.Collections.Generic;
using TaskManager.Models;

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

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) &&
                !referer.Contains("/TaskItem/Upsert", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ReturnUrl = referer;
            }


            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == id);
            var teamFromDb = _unitOfWork.Team.Get(t => t.Id == teamId);
            var userList = new List<AppUser>();

            var users = _unitOfWork.AppUser.GetAll(u => u.Teams.Contains(teamFromDb));
            foreach (var u in users)
            {
                userList.Add(u);
            }

            if (id == 0 || id == null)
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
                    SelectedUserId = null
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
                    SelectedUserId = taskItem?.AppUserId
                };

                return View(task);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ToDoVM taskItemVM, string? returnUrl)
        {
            // assegna l'utente selezionato dall'admin
            taskItemVM.TaskToDo.AppUserId = string.IsNullOrWhiteSpace(taskItemVM.SelectedUserId) ? null : taskItemVM.SelectedUserId;
            if (taskItemVM.TaskToDo.DueDate.HasValue)
            {
                var hour = taskItemVM.TaskToDo.DueDate.Value.Hour;
                if (hour < 8 || hour > 19)
                {
                    ModelState.AddModelError("TaskToDo.DueDate", "L'orario deve essere compreso tra le 08:00 e le 19:00.");
                }
            }
            if (taskItemVM.TaskToDo.DueDate == null)
            {
                ModelState.AddModelError("TaskToDo.DueDate", "Inserire la data di scadenza.");
            }
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
                            // mantiene l'AppUserId corrente nel DB (precedente assegnazione)
                            AppUserId = _unitOfWork.TaskItem.Get(t => t.Id == taskItemVM.TaskToDo.Id).AppUserId
                        };
                        _unitOfWork.History.Add(history);
                    }

                    // assegna gli oggetti Status/Priority e aggiorna TaskItem (incluso AppUserId già impostato sopra)
                    taskItemVM.TaskToDo.Status = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId);
                    taskItemVM.TaskToDo.Priority = _unitOfWork.Priority.Get(p => p.Id == taskItemVM.PrioritySelectedId);
                    _unitOfWork.TaskItem.Update(taskItemVM.TaskToDo);
                }

                _unitOfWork.Save();
                taskItemVM.TaskToDo.Status = _unitOfWork.Status.Get(s => s.Id == taskItemVM.StatusSelectedId);
                taskItemVM.TaskToDo.Priority = _unitOfWork.Priority.Get(s => s.Id == taskItemVM.PrioritySelectedId);
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Team");
            }

            // se invalid, ricarica le liste prima di tornare alla view
            var teamFromDb = _unitOfWork.Team.Get(t => t.Id == taskItemVM.TaskToDo.TeamId);
            var usersReload = _unitOfWork.AppUser.GetAll(u => u.Teams.Contains(teamFromDb)).ToList();
            taskItemVM.Users = usersReload.Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
            taskItemVM.PriorityList = _unitOfWork.Priority.GetAll().Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() });
            taskItemVM.StatusList = _unitOfWork.Status.GetAll().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            return View(taskItemVM);
        }
    }
}
