using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManager.Services.Services;
using Xunit;

namespace Test.ServicesTest
{
    public class HistoryServiceTest
    {
        private readonly HistoryService _historyService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;

        public HistoryServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _httpContextMock = new Mock<IHttpContextAccessor>();
            _historyService = new HistoryService(_httpContextMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async void GetAllByTaskId_ValidTaskId_ReturnsHistory()
        {
            // Arrange
            int taskId = 1;
            var histories = new List<History>
            {
                new History { Id = 1, TaskItemId = taskId, FromStatus = "History 1" ,ToStatus = "History"},
                new History { Id = 2, TaskItemId = taskId, FromStatus = "History 2" ,ToStatus = "History" }
            };
            _unitOfWorkMock.Setup(uow => uow.History.GetAll(It.IsAny<Expression<Func<History, bool>>>(), It.IsAny<string>()))
                .Returns(histories);
            _httpContextMock.Setup(h => h.HttpContext.User.IsInRole(It.IsAny<string>())).Returns(true);
            // Act
            var result = _historyService.GetAllByTaskId(taskId);
            // Assert
            Assert.Equal(histories, result);
            Assert.NotNull(result);
            Assert.IsType<List<History>>(result);
        }

        [Fact]
        public void GetAllByTaskId_UserNotInRole_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            int taskId = 1;
            var histories = new List<History>();
            _unitOfWorkMock.Setup(uow => uow.History.GetAll(It.IsAny<Expression<Func<History, bool>>>(), It.IsAny<string>()))
                .Returns(histories);
            _httpContextMock.Setup(h => h.HttpContext.User.IsInRole(It.IsAny<string>())).Returns(false);
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _historyService.GetAllByTaskId(taskId));
        }
    }
}
