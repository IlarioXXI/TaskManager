using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.Services.Services;
using Xunit;

namespace Test.ServicesTest
{
    public class TeamServiceTest
    {
        private readonly TeamService _teamService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserClaimService> _userClaimsServiceMOck;
        private readonly Mock<ILogger<TeamService>> _loggerMock;

        public TeamServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userClaimsServiceMOck = new Mock<IUserClaimService>();
            _loggerMock = new Mock<ILogger<TeamService>>();
            _teamService = new TeamService(_unitOfWorkMock.Object, _userClaimsServiceMOck.Object, _loggerMock.Object);
        }

        [Fact]
        public void Delete_ok()
        {
            _unitOfWorkMock.Setup(uow => uow.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Team { Id = 1, Name = "Team 1" });
           _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "Admin")
                });
            _unitOfWorkMock.Setup(uow => uow.Team.Remove(It.IsAny<Team>()));
            var result = _teamService.Delete(1);
            Assert.IsType<bool>(result);
            Assert.True(result);
        }

        [Fact]
        public void Delete_notFound()
        {
            _unitOfWorkMock.Setup(uow => uow.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((Team)null);

            Assert.Throws<Exception>(() => _teamService.Delete(1));
        }

        [Fact]
        public void Delete_UserNotInRole_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Team.Get(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(new Team()
            {
                Id = 1,
                Name = "Test",
            });
            _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_User)
                });
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _teamService.Delete(1));
        }

        [Fact]
        public void GetAllMyTeams_ok()
        {
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team 1" },
                new Team { Id = 2, Name = "Team 2" }
            };
            _userClaimsServiceMOck.Setup(uow => uow.GetUserId()).Returns("user-id-test");
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "user-id-test", Teams = teams });
            _unitOfWorkMock.Setup(uow => uow.Team.GetAll(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>()))
                .Returns(teams);
            var result = _teamService.GetAllMyTeams();
            Assert.Equal(teams, result);
            Assert.NotNull(result);
            Assert.IsType<List<Team>>(result);
        }

        [Fact]  
        public void GetAll_ok()
        {
            _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim> { new Claim(ClaimTypes.Role, "Admin") });
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Team 1" },
                new Team { Id = 2, Name = "Team 2" }
            };
            
            _unitOfWorkMock.Setup(uow => uow.Team.GetAll(It.IsAny<Expression<Func<Team, bool>>>(), It.IsAny<string>()))
                .Returns(teams);
            var result = _teamService.GetAll();
            Assert.Equal(teams, result);
            Assert.NotNull(result);
            Assert.IsType<List<Team>>(result);
        }

        [Fact]
        public void GetAll_UserNotInRole_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_User)
                });
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _teamService.GetAll());
        }

        [Fact]
        public void Upsert_ok_add()
        {
            var team = new Team { Id = 0, Name = "Team 1" };
            _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "Admin")
                });
            _userClaimsServiceMOck.Setup(uow => uow.GetUserId()).Returns("test-user-id");
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });

            _unitOfWorkMock.Setup(uow => uow.Team.Add(It.IsAny<Team>()));
            var result = _teamService.Upsert(team);
            Assert.NotNull(result);
            Assert.IsType<Team>(result);
            Assert.Equal(team, result);
        }

        [Fact]
        public void Upsert_UserNotInRole_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var team = new Team { Id = 0, Name = "Team 1" };
            _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim> { new Claim(ClaimTypes.Role, "User") });
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _teamService.Upsert(team));
        }
        [Fact]
        public void Upsert_ok_update()
        {
            var team = new Team { Id = 1, Name = "Team 1" };
            _userClaimsServiceMOck.Setup(uow => uow.GetUserId()).Returns("test-user-id");
            _userClaimsServiceMOck.Setup(uow => uow.GetClaims()).Returns(new List<Claim> { new Claim(ClaimTypes.Role, "Admin") });
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });
            _unitOfWorkMock.Setup(uow => uow.Team.Update(It.IsAny<Team>()));
            var result = _teamService.Upsert(team);
            Assert.NotNull(result);
            Assert.IsType<Team>(result);
            Assert.Equal(team, result);
        }

    }
}
