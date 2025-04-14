using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.Controllers;
using TaskManagerWEB.Api.ViewModels;
using Xunit;
using TeamVM = TaskManagerWEB.Api.ViewModels.TeamVM;

namespace Test.ControllerTest
{
    public class TeamControllerTests
    {
        private readonly TeamController _controller;
        private readonly Mock<IValidator<TaskManagerWEB.Api.ViewModels.TeamVM>> _validatorMock;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ITeamService> _teamService;

        public TeamControllerTests()
        {
            _validatorMock = new Mock<IValidator<TaskManagerWEB.Api.ViewModels.TeamVM>>();
            _mapper = new Mock<IMapper>();
            _teamService = new Mock<ITeamService>();
            _controller = new TeamController(_teamService.Object,_validatorMock.Object,_mapper.Object);
        }


        [Fact]
        public void GetAll_ShouldReturnOkResult_WhenUserIsAdmin()
        {

            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team A" },
                new Team { Id = 2, Name = "Team B" }
            };

            _teamService.Setup(u => u.GetAll()).Returns(teams);
            var teamsVM = new List<TeamVM>
            {
                new TeamVM { Id = 1, Name = "Team A" },
                new TeamVM { Id = 2, Name = "Team B" }
            };
            _mapper.Setup(m => m.Map<IEnumerable<Team>, IEnumerable<TeamVM>>(teams)).Returns(teamsVM);
            var result = _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(teamsVM, okResult.Value);

        }


        [Fact]
        public void GetAllMyTeams_ok()
        {
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team A" },
                new Team { Id = 2, Name = "Team B" }
            };
            var teamsVM = new List<TeamVM>
            {
                new TeamVM { Id = 1, Name = "Team A" },
                new TeamVM { Id = 2, Name = "Team B" }
            };
            _teamService.Setup(u => u.GetAllMyTeams()).Returns(teams);
            _mapper.Setup(m => m.Map<IEnumerable<Team>, IEnumerable<TeamVM>>(teams)).Returns(teamsVM);
            var result = _controller.GetAllMyTeams();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(teamsVM, okResult.Value);
        }


        [Fact]
        public async Task UpsertAsync_ShouldReturnOkResultAsync_Add()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<TeamVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var teamVM = new TeamVM
            {
               Id = 0, Name = "Team A" 
            };
            var team = new Team
            {
                Id = 0,
                Name = "Team A"
            };
            _mapper.Setup(m => m.Map<TeamVM, Team>(teamVM)).Returns(team);
            _teamService.Setup(t=>t.Upsert(It.IsAny<Team>())).Returns(team);
            _mapper.Setup(m => m.Map<Team, TeamVM>(team)).Returns(teamVM);
            var result = await _controller.UpsertAsync(teamVM);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(teamVM, okResult.Value);
        }

        [Fact]
        public async Task UpsertAsync_BadRequest()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<TeamVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure("Team.Name", "Name is required")
                }));
            var result = await _controller.UpsertAsync(new TeamVM());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }



        [Fact]
        public void Delete_BadRequest()
        {
            _teamService.Setup(u => u.Delete(It.IsAny<int>())).Returns(false);
            var result = _controller.Delete(1);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }



        [Fact]
        public void Delete_OkResult()
        {
            _teamService.Setup(u => u.Delete(It.IsAny<int>())).Returns(true);
            var result = _controller.Delete(1);
            var okResult = Assert.IsType<OkResult>(result);
        }
    }
}
