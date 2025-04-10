using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.Models;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.Controllers;
using Xunit;

namespace Test
{
    public class TeamControllerTests
    {
        private readonly TeamController _controller;
        private readonly Mock<IValidator<TeamVM>> _validatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<TeamController>> _loggerMock;

        public TeamControllerTests()
        {
            _validatorMock = new Mock<IValidator<TeamVM>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<TeamController>>();
            _controller = new TeamController(_loggerMock.Object, _unitOfWorkMock.Object, _validatorMock.Object);
        }


        [Fact]
        public void GetAll_ShouldReturnOkResult_WhenUserIsAdmin()
        {

            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team A" },
                new Team { Id = 2, Name = "Team B" }
            };

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };

            _unitOfWorkMock.Setup(u => u.Team.GetAll(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>())).Returns(teams);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(teams, okResult.Value);
        }

        [Fact]
        public void GetAll_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };

            var result = _controller.GetAll();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void GetAllMyTeams_ShouldReturnOkResult_WhenUserIsAuthenticated()
        {

            var userId = "test-user-id";
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team A" },
                new Team { Id = 2, Name = "Team B" }
            };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }))
            };
            var user = new AppUser { Id = userId };
            _unitOfWorkMock.Setup(u => u.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(),It.IsAny<bool>())).Returns(user);
            _unitOfWorkMock.Setup(u => u.Team.GetAll(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>())).Returns(teams);

            var result = _controller.GetAllMyTeams();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(teams, okResult.Value);
        }


        [Fact]
        public async Task UpsertAsync_ShouldReturnOkResultAsync_Add()
        {
            var team = new Team { Id = 0, Name = "Team A" };
            var teamVM = new TeamVM { Team = team };
            var validationResult = new FluentValidation.Results.ValidationResult();

            var userId = "test-user-id";
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            var user = new AppUser { Id = userId };
            _unitOfWorkMock.Setup(u => u.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(user);

            _validatorMock.Setup(v => v.ValidateAsync(teamVM, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _unitOfWorkMock.Setup(u => u.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(team);


            var result = await _controller.UpsertAsync(teamVM);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(team, okResult.Value);
        }

        [Fact]
        public async Task UpsertAsync_ShouldReturnOkResultAsync_Update()
        {
            var team = new Team { Id = 1, Name = "Team A" };
            var teamVM = new TeamVM { Team = team };
            var validationResult = new FluentValidation.Results.ValidationResult();

            var userId = "test-user-id";
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            var user = new AppUser { Id = userId };
            _unitOfWorkMock.Setup(u => u.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(user);

            _validatorMock.Setup(v => v.ValidateAsync(teamVM, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _unitOfWorkMock.Setup(u => u.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(team);


            var result = await _controller.UpsertAsync(teamVM);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(team, okResult.Value);
        }


        [Fact]
        public async Task UpsertAsync_ShouldReturnBadRequest_WhenValidationFails()
        {
            var team = new Team { Id = 0, Name = "Team A" };
            var teamVM = new TeamVM { Team = team };
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Team.Name", "Name is required")
            });
            _validatorMock.Setup(v => v.ValidateAsync(teamVM, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            var result = await _controller.UpsertAsync(teamVM);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task UpsertAsync_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
        {
            var team = new Team { Id = 0, Name = "Team A" };
            var teamVM = new TeamVM { Team = team };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            _validatorMock.Setup(v => v.ValidateAsync(teamVM, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            var result = await _controller.UpsertAsync(teamVM);
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void Delete_BadRequest()
        {
            var team = new Team();
            team = null;
            _unitOfWorkMock.Setup(u => u.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(team);
            var result = _controller.Delete(1);
            var notFoundRequestResult = Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void Delete_Unauthorized()
        {
            var userId = "test-user-id";
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            var user = new AppUser { Id = userId };
            _unitOfWorkMock.Setup(u => u.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(user);
            var team = new Team() { Id = 1, Name = "Team A" };
            _unitOfWorkMock.Setup(u => u.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(team);
            var result = _controller.Delete(1);
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void Delete_OkResult()
        {
            var userId = "test-user-id";
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            var user = new AppUser { Id = userId };
            _unitOfWorkMock.Setup(u => u.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(user);
            var team = new Team() { Id = 1, Name = "Team A" };
            _unitOfWorkMock.Setup(u => u.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(team);
            var result = _controller.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(team, okResult.Value);
        }
    }
}
