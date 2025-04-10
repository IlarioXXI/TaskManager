using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.Models;
using TaskManagerWeb.Models;

namespace TaskManagerWeb.Areas.User.Controllers
{
    [Area("User")]
    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TeamController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Upsert(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            TeamVM teamVM = new()
            {
                Team = new Team(),
                Users = _unitOfWork.AppUser.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                SelectedUserIds = new List<string>(),
                TaskItems = new List<TaskItem>(),
                
            };
            foreach (var task in teamVM.TaskItems)
            {
                task.AppUserId = userId;
            }
            if (id != null && id != 0)
            {
                teamVM.Team = _unitOfWork.Team.Get(u => u.Id == id, includeProperties: "TaskItems,Users");
                teamVM.SelectedUserIds = teamVM.Team.Users.Select(u => u.Id).ToList();
            }

            return View(teamVM);
        }
        [HttpPost]
        public IActionResult Upsert(TeamVM teamVM)
        {
            
            if (ModelState.IsValid)
            {
                if (teamVM.Team.Id == 0)
                {
                    _unitOfWork.Team.Add(teamVM.Team);
                }
                else
                {
                    _unitOfWork.Team.Update(teamVM.Team);
                }
                if (teamVM.SelectedUserIds != null)
                {
                    teamVM.Team.Users = _unitOfWork.AppUser.GetAll()
                    .Where(u => teamVM.SelectedUserIds.Contains(u.Id))
                    .ToList();
                }

                _unitOfWork.Save();

                return RedirectToAction("Index", "Home");
            }

            // Ripopola gli utenti in caso di errore
            teamVM.Users = _unitOfWork.AppUser.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(teamVM);
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var teamList = _unitOfWork.Team.GetAll(t => t.Users.FirstOrDefault(u => u.Id == claim).Id == claim, includeProperties: "TaskItems,Users");
            foreach (var t in teamList)
            {
                foreach (var ti in t.TaskItems)
                {
                    ti.Status = _unitOfWork.Status.Get(s=>s.Id == ti.StatusId);
                    ti.Priority = _unitOfWork.Priority.Get(p=>p.Id == ti.PriorityId);
                }
            }
            List<TeamVM> teamVMs = new List<TeamVM>();
            
            foreach (var team in teamList)
            {
                TeamVM teamVM = new()
                {
                    Team = _unitOfWork.Team.Get(t=>t.Id == team.Id,includeProperties:"TaskItems,Users"),
                    Users = _unitOfWork.AppUser.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    TaskItems = _unitOfWork.TaskItem.GetAll(t=>t.TeamId == team.Id,includeProperties:"Comments"),
                };
                teamVMs.Add(teamVM);
            }
            
            return View(teamVMs);

        }


        public IActionResult Delete(int id)
        {
            var teamToDelete = _unitOfWork.Team.Get(t=>t.Id == id);
            _unitOfWork.Team.Remove(teamToDelete);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
