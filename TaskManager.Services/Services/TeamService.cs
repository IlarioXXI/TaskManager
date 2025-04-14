using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
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
            if (_userClaimService.GetClaims().FirstOrDefault(c=>c.Type == ClaimTypes.Role)?.Value != SD.Role_Admin)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
            var teams = _unitOfWork.Team.GetAll();
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
                _unitOfWork.Team.Update(team);
                _unitOfWork.Save();
                _logger.LogInformation("Team ({teamName}) updated successfully by : {email}", team.Name, user.Email);
                return team;
            }
        }
    }
}
