using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Extensions;
using TaskManager.Services.Interfaces;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.ViewModels;
using TeamVM = TaskManagerWEB.Api.ViewModels.TeamVM;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IValidator<TeamVM> _validator;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService,IValidator<TeamVM> validation,IMapper mapper)
        {
            _teamService = teamService;
            _validator = validation;
            _mapper = mapper;   
        }


        [HttpGet("getAll")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
        {
            var teams = _teamService.GetAll();
            var result = _mapper.Map<IEnumerable<Team>,IEnumerable<TeamVM>>(teams);
            return Ok(result);
        }

        [HttpGet("getAllMyTeams")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        public IActionResult GetAllMyTeams()
        {
            var teams = _teamService.GetAllMyTeams();
            var result = _mapper.Map<IEnumerable<Team>, IEnumerable<TeamVM>>(teams);
            return Ok(result);
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
            var teamToUpdate = _mapper.Map<TeamVM, Team>(team);
            var result = _teamService.Upsert(teamToUpdate);
            var resultVM = _mapper.Map<Team, TeamVM>(result);
            return Ok(resultVM);

        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete(int id)
        {
            var result = _teamService.Delete(id);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Error deleting team");
            }
        }
    }
}
