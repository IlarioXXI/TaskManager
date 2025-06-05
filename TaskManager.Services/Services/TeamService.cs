using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.Security.Claims;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserClaimService _userClaimService;
        private readonly ILogger<TeamService> _logger;
        public TeamService(IUnitOfWork unitOfWork, IUserClaimService userClaimService, ILogger<TeamService> logger)
        {
            _unitOfWork = unitOfWork;
            _userClaimService = userClaimService;
            _logger = logger;
        }
        public bool Delete(int id)
        {
            var team = _unitOfWork.Team.Get(t => t.Id == id);
            if (team == null)
            {
                throw new Exception("Team not found");
            }
            if (_userClaimService.GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != SD.Role_Admin)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            _unitOfWork.Team.Remove(team);
            _unitOfWork.Save();
            _logger.LogInformation("Team ({teamName}) deleted successfully", team.Name);
            return true;
        }

        public IEnumerable<Team> GetAll()
        {
            if (_userClaimService.GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != SD.Role_Admin)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            var teams = _unitOfWork.Team.GetAll(includeProperties:"Users,TaskItems,TaskItems.Comments");
            
            return teams;
        }

        public IEnumerable<Team> GetAllMyTeams()
        {
            var userId = _userClaimService.GetUserId();
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            var teams = _unitOfWork.Team.GetAll(t => t.Users.Contains(user));
            return teams;
        }

        public Team Upsert(Team team)
        {
            

            if (!team.TaskItems.IsNullOrEmpty())
            {
                var taskItemsInTeam = new List<TaskItem>();
                foreach (var ti in team.TaskItems)
                {
                    var taskItemInTeam = _unitOfWork.TaskItem.Get(t => t.Id == ti.Id,includeProperties: null, tracked: true);
                    taskItemsInTeam.Add(taskItemInTeam);
                }
                team.TaskItems = taskItemsInTeam;
            }
            if (!team.Users.IsNullOrEmpty())
            {
                var usersInTeam = new List<AppUser>();
                foreach (var u in team.Users)
                {
                    var userInTeam = _unitOfWork.AppUser.Get(x=>x.Id == u.Id,includeProperties:null,tracked:true);
                    usersInTeam.Add(userInTeam);
                }
                team.Users = usersInTeam;
            }

            if (team == null)
            {
                throw new Exception("Team is null");
            }
            if (_userClaimService.GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != SD.Role_Admin)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            var userId = _userClaimService.GetUserId();     
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (team.Id == 0 || team.Id == null)
            {
            
                _unitOfWork.Team.Add(team);
                _unitOfWork.Save();
                _logger.LogInformation("Team ({teamName}) created successfully by : {email}", team.Name, user.Email);
                return team;
            }
            else
            {
                var existingTeam = _unitOfWork.Team.Get(t => t.Id == team.Id,includeProperties:"Users",true);

                if (existingTeam == null)
                {
                    throw new Exception("Team not found");
                }

                existingTeam.Name = team.Name;
                if (!existingTeam.Users.IsNullOrEmpty())
                {
                    var existingUserIds = existingTeam.Users.Select(u => u.Id).ToList();

                    var newUserIds = team.Users.Select(u => u.Id).ToList();

                    foreach (var newUserId in newUserIds.Except(existingUserIds))
                    {
                        var newUser = _unitOfWork.AppUser.Get(u => u.Id == newUserId);
                        if (newUser != null)
                        {
                            existingTeam.Users.Add(newUser);
                        }
                    }

                    foreach (var removedUserId in existingUserIds.Except(newUserIds))
                    {
                        var removedUser = existingTeam.Users.FirstOrDefault(u => u.Id == removedUserId);
                        if (removedUser != null)
                        {
                            existingTeam.Users.Remove(removedUser);
                        }
                    }
                }
                else
                {
                    existingTeam.Users = team.Users;
                }

                    existingTeam.TaskItems = team.TaskItems;

                _unitOfWork.Team.Update(existingTeam);
                _unitOfWork.Save();
                _logger.LogInformation("Team ({teamName}) updated successfully by : {email}", team.Name, user.Email);
                return existingTeam;
            }
        }
    }
}
