using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.Models;
using TaskManagerWEB.Api.Controllers;
using Xunit;

namespace Test
{
    public class HistoryControllerTests
    {
        private readonly HistoryController _controller;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public HistoryControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new HistoryController(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ValidTaskId_ReturnsOkResult()
        {
            // Arrange
            int taskId = 1;
            var historyItems = new List<History>
            {
                new History { Id = 1, TaskItemId = taskId, FromStatus = "History 1",ToStatus = "ciao" },
                new History { Id = 2, TaskItemId = taskId, FromStatus = "History 2", ToStatus = "ciao" }
            };

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };

            _unitOfWorkMock.Setup(uow => uow.History.GetAll(It.IsAny<Expression<Func<History, bool>>>(),It.IsAny<string>()))
                .Returns(historyItems);

            // Act
            var result = _controller.GetAll(taskId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(historyItems, okResult.Value);
        }

        [Fact]
        public void GetAll_ValidTaskId_ReturnsUnauthorized()
        {
            // Arrange
            int taskId = 1;
            var historyItems = new List<History>
            {
                new History { Id = 1, TaskItemId = taskId, FromStatus = "History 1",ToStatus = "ciao" },
                new History { Id = 2, TaskItemId = taskId, FromStatus = "History 2", ToStatus = "ciao" }
            };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.History.GetAll(It.IsAny<Expression<Func<History, bool>>>(), It.IsAny<string>()))
                .Returns(historyItems);
            // Act
            var result = _controller.GetAll(taskId);
            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void GetAll_EmptyHistory_ReturnsOkResult()
        {
            // Arrange
            int taskId = 1;
            var historyItems = new List<History>();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.History.GetAll(It.IsAny<Expression<Func<History, bool>>>(), It.IsAny<string>()))
                .Returns(historyItems);
            // Act
            var result = _controller.GetAll(taskId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(historyItems, okResult.Value);
        }
    }
}
