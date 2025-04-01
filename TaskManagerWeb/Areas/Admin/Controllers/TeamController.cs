using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManagerWeb.Models;

namespace TaskManagerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public TeamController(IUnitOfWork unitOfWork, AppDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Upsert(int? id)
        {
            TeamVM teamVM = new()
            {
                Team = new Team(),
                Users = _unitOfWork.AppUser.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                SelectedUserIds = new List<string>(),
                TaskItems = new List<TaskItem>()
            };

            if (id != null && id != 0)
            {
                teamVM.Team = _unitOfWork.Team.Get(u => u.Id == id, includeProperties: "TaskItems,Users");
                teamVM.SelectedUserIds = teamVM.Team.Users.Select(u => u.Id).ToList(); // Popola gli ID selezionati
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
                    _unitOfWork.Team.UpdateTeamInUsers(teamVM.Team, teamVM.SelectedUserIds);
                }
                //if (teamVM.SelectedUserIds != null)
                //{
                //    teamVM.Team.Users = _unitOfWork.AppUser.GetAll()
                //    .Where(u => teamVM.SelectedUserIds.Contains(u.Id))
                //    .ToList();
                //}

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
            var teamList = _unitOfWork.Team.GetAll(includeProperties: "TaskItems,Users");
            foreach (var t in teamList)
            {
                foreach (var ti in t.TaskItems)
                {
                    ti.Status = _unitOfWork.Status.Get(s => s.Id == ti.StatusId);
                    ti.Priority = _unitOfWork.Priority.Get(p => p.Id == ti.PriorityId);
                }
            }
            List<TeamVM> teamVMs = new List<TeamVM>();

            foreach (var team in teamList)
            {
                TeamVM teamVM = new()
                {
                    Team = team,
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
            var teamToDelete = _unitOfWork.Team.Get(t => t.Id == id);
            _unitOfWork.Team.Remove(teamToDelete);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
