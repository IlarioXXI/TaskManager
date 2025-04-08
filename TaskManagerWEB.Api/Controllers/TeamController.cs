using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManagerWeb.Models;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TeamController : Controller
    {
        private readonly ILogger<TeamController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<TeamVM> _validator;

        public TeamController(ILogger<TeamController> logger, IUnitOfWork unitOfWork, IValidator<TeamVM> validation)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validator = validation;
        }


        [HttpGet("getAll")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
        {
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            var teams = _unitOfWork.Team.GetAll();
            return Ok(teams);
        }

        [HttpGet("getAllMyTeams")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        public IActionResult GetAllMyTeams()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            var teams = _unitOfWork.Team.GetAll(t => t.Users.Contains(user));
            return Ok(teams);

        }

        [HttpPost("upsert")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpsertAsync(TeamVM team)
        {
            var resultValidation = await _validator.ValidateAsync(team);
            if (!resultValidation.IsValid)
            {
                resultValidation.AddToModelState(this.ModelState);
                var errors = new List<string>();
                foreach (var error in resultValidation.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
                return BadRequest(errors);
            }
            if (team.Team == null)
            {
                return BadRequest();
            }
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (team.Team.Id == 0 || team.Team.Id == null)
            {
                _unitOfWork.Team.Add(team.Team);
                _unitOfWork.Save();
                _logger.LogInformation("Team ({teamName}) created successfully by : {email}", team.Team.Name, user.Email);
                return Ok(team.Team);
            }
            else
            {
                var existingTeam = _unitOfWork.Team.Get(t => t.Id == team.Team.Id);
                existingTeam.Name = team.Team.Name;
                if (team.SelectedUserIds != null)
                {
                    _unitOfWork.Team.UpdateTeamInUsers(existingTeam, team.SelectedUserIds);
                    _unitOfWork.Save();
                    _logger.LogInformation("Team ({teamName}) updated successfully by : {email}", team.Team.Name, user.Email);
                    return Ok(existingTeam);
                }
                _unitOfWork.Team.Update(existingTeam);
                _unitOfWork.Save();
                _logger.LogInformation("Team ({teamName}) updated successfully by : {email}", team.Team.Name, user.Email);
                return Ok(existingTeam);
            }
        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete(int id)
        {
            var team = _unitOfWork.Team.Get(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            _unitOfWork.Team.Remove(team);
            _unitOfWork.Save();
            _logger.LogInformation("Team ({teamName}) deleted successfully", team.Name);
            return Ok(team);
        }
    }
}
