using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.Services.Services;
using Xunit;

namespace Test.ServicesTest
{
    public class TaskItemServiceTest
    {
        private readonly TaskItemService _taskItemService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserClaimService> _userClaimServiceMock;
        private readonly Mock<ILogger<TaskItemService>> _loggerMock;

        public TaskItemServiceTest()
        {
            _userClaimServiceMock = new Mock<IUserClaimService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<TaskItemService>>();
            _taskItemService = new TaskItemService(_unitOfWorkMock.Object, _userClaimServiceMock.Object,_loggerMock.Object);
        }

        [Fact]
        public void GetAll_ValidUserId_ReturnsTaskItems()
        {
            // Arrange
            var userId = "test-user-id";
            var taskItems = new List<TaskItem>
            {
                new TaskItem { Id = 1, AppUserId = userId, Title = "Task 1" },
                new TaskItem { Id = 2, AppUserId = userId, Title = "Task 2" }
            };
            _userClaimServiceMock
                .Setup(uc => uc.GetUserId())
                .Returns(userId);
            _userClaimServiceMock
                .Setup(uc=>uc.GetClaims())
                .Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_Admin)
                });
            _unitOfWorkMock.Setup(uow => uow.TaskItem.GetAll(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>()))
                .Returns(taskItems);
            // Act
            var result = _taskItemService.GetAll();
            // Assert
            Assert.Equal(taskItems, result);
        }
        [Fact]
        public void Delete_ok()
        {
            _userClaimServiceMock.Setup(uc => uc.GetUserId()).Returns("test-user-id");
            _userClaimServiceMock.Setup(uc => uc.GetClaims())
                .Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_Admin)
                });
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(new TaskItem { Id = 1 });

            _unitOfWorkMock.Setup(uow => uow.TaskItem.Remove(It.IsAny<TaskItem>()));
            var result = _taskItemService.Delete(1);
            _unitOfWorkMock.Verify(uow => uow.TaskItem.Remove(It.IsAny<TaskItem>()), Times.Once);
            Assert.IsType<bool>(result);
            Assert.True(result);
        }

        [Fact]
        public void Delete_UserNotInRole_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userClaimServiceMock.Setup(uc => uc.GetUserId()).Returns("test-user-id");
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });
            _userClaimServiceMock.Setup(uc => uc.GetClaims()).Returns(new List<Claim>
            {
                new Claim(ClaimTypes.Role, SD.Role_User)
            });
            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _taskItemService.Delete(1));
        }

        [Fact]
        public void Delete_TaskNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            _userClaimServiceMock.Setup(uc => uc.GetUserId()).Returns("test-user-id");
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });
            _userClaimServiceMock.Setup(uc => uc.GetClaims()).Returns(new List<Claim>
                {
                new Claim(ClaimTypes.Role, SD.Role_Admin)
            });
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((TaskItem)null);
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _taskItemService.Delete(1));
        }

        [Fact]
        public void GetById_ok()
        {
            // Arrange
            var taskItemId = 1;
            var taskItem = new TaskItem { Id = taskItemId, Title = "Task 1" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(taskItem);
            // Act
            var result = _taskItemService.GetById(taskItemId);
            // Assert
            Assert.Equal(taskItem, result);
            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
        }

        [Fact]
        public void Upsert_ok_Add()
        {
            _userClaimServiceMock.Setup(uc => uc.GetUserId()).Returns("test-user-id");
            _unitOfWorkMock.Setup(uof=>uof.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });
            var taskItem = new TaskItem { Id = 0, Title = "Task 1" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Add(It.IsAny<TaskItem>()));
            _unitOfWorkMock.Setup(uow => uow.Save());
            var result = _taskItemService.Upsert(taskItem);
            Assert.Equal(taskItem, result);
            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
        }
        [Fact]
        public void Upsert_ok_Update()
        {
            _userClaimServiceMock.Setup(uc => uc.GetUserId()).Returns("test-user-id");
            _unitOfWorkMock.Setup(uof => uof.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = "test-user-id" });
            var taskItem = new TaskItem { Id = 1, Title = "Task 1" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(taskItem);
            _unitOfWorkMock.Setup(uow => uow.Status.Get(It.IsAny<Expression<Func<Status, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Status { Id = 1, Name = "In Progress" });
            _unitOfWorkMock.Setup(uow => uow.Priority.Get(It.IsAny<Expression<Func<Priority, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Priority { Id = 1, Name = "High" });
            var history = new History
            {
                FromStatus = "New",
                ToStatus = "In Progress",
                TaskItemId = taskItem.Id
            };
            _unitOfWorkMock.Setup(uow => uow.History.Add(It.IsAny<History>()));
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Update(It.IsAny<TaskItem>()));
            _unitOfWorkMock.Setup(uow => uow.Save());
            var result = _taskItemService.Upsert(taskItem);
            Assert.Equal(taskItem, result);
            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
        }
    }
}
