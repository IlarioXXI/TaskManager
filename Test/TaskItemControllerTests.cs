using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.Controllers;
using Xunit;

namespace Test
{
    public class TaskItemControllerTests
    {
        private readonly Mock<IValidator<ToDoVM>> _validatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<TaskItemController>> _loggerMock;
        private readonly TaskItemController _controller;

        public TaskItemControllerTests()
        {
            _validatorMock = new Mock<IValidator<ToDoVM>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<TaskItemController>>();
            _controller = new TaskItemController(_loggerMock.Object, _unitOfWorkMock.Object, _validatorMock.Object);
        }

        [Fact]
        public void GetAll_Ok()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };

            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });

            var taskItems = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Test task 1" },
                new TaskItem { Id = 2, Title = "Test task 2" }
            };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.GetAll(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>()))
                .Returns(taskItems);

            var result = _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<TaskItem>>(okResult.Value);
            Assert.Equal(taskItems, returnValue);
        }

        [Fact]
        public void GetAll_Unauthorized()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            var result = _controller.GetAll();
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void GetById_OK()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });

            var taskId = 1;
            var taskItem = new TaskItem { Id = taskId, Title = "Test task" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(taskItem);
            var result = _controller.GetById(taskId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TaskItem>(okResult.Value);
            Assert.Equal(taskItem, returnValue);
        }

        [Fact]
        public void GetById_Unauthorized()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            var taskId = 1;
            var result = _controller.GetById(taskId);
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Upsert_ValidTaskItem_ReturnsOk_Add()
        {
            var taskItem = new TaskItem
            {
                AppUserId = "AppUser-UserValidation-id",
                StatusId = 2,
                PriorityId = 1,
                Description = "Completa la documentazione del progetto.",
                DueDate = DateTime.Now.AddDays(7),
                TaskNotification = DateTime.Now,
                Title = "Documentazione",
                TeamId = 1,
                Id = 0
            };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ToDoVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });

            _unitOfWorkMock.Setup(uow => uow.TaskItem.Add(It.IsAny<TaskItem>()));

            var taskItemVm = new ToDoVM()
            {
                TaskToDo = taskItem
            };

            var teamId = 1;
            var result = _controller.Upsert(teamId, taskItemVm);
            var okResult = Assert.IsType<Task<IActionResult>>(result);
            var returnValue = Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.Equal(taskItem, returnValue.Value);
        }


        [Fact]
        public async Task Upsert_ValidTaskItem_ReturnsOk_Update()
        {
            var taskItem = new TaskItem
            {
                AppUserId = "AppUser-UserValidation-id",
                StatusId = 2,
                PriorityId = 1,
                Description = "Completa la documentazione del progetto.",
                DueDate = DateTime.Now.AddDays(7),
                TaskNotification = DateTime.Now,
                Title = "Documentazione",
                TeamId = 1,
                Id = 1
            };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ToDoVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Add(It.IsAny<TaskItem>(),It.IsAny<int>(),It.IsAny<int>()));
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(taskItem);
            _unitOfWorkMock.Setup(uow=>uow.Status.Get(It.IsAny<Expression<Func<Status, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Status { Id = 2, Name = "In Progress" });
            _unitOfWorkMock.Setup(uow => uow.Priority.Get(It.IsAny<Expression<Func<Priority, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Priority { Id = 1, Name = "High" });

            var taskItemVm = new ToDoVM()
            {
                TaskToDo = taskItem
            };
            var teamId = 1;
            var result = _controller.Upsert(teamId, taskItemVm);
            var okResult = Assert.IsType<Task<IActionResult>>(result);
            var returnValue = Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.Equal(taskItem, returnValue.Value);
        }

        [Fact]
        public async void Upsert_InvalidTaskItem_ReturnsBadRequest()
        {
            var taskItem = new TaskItem
            {
                AppUserId = "AppUser-UserValidation-id",
                StatusId = 2,
                PriorityId = 1,
                Description = "Completa la documentazione del progetto.",
                DueDate = DateTime.Now.AddDays(7),
                TaskNotification = DateTime.Now,
                Title = "Documentazione",
                TeamId = 1,
                Id = 0
            };
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ToDoVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            var taskItemVm = new ToDoVM()
            {
                TaskToDo = taskItem
            };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Add(It.IsAny<TaskItem>(), It.IsAny<int>(), It.IsAny<int>()));
            var teamId = 1;
            var result = _controller.Upsert(teamId, taskItemVm);
            var badRequestResult = Assert.IsType<Task<IActionResult>>(result);
            var returnValue = Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
        }

        [Fact]
        public void Delete_Ok()
        {
            var taskId = 1;
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new TaskItem { Id = taskId });
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });

            var result = _controller.Delete(taskId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Delete successfully", returnValue);
        }

        [Fact]
        public void Delete_Unauthorized()
        {
            var taskId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            var result = _controller.Delete(taskId);
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]  
        public void Delete_NotFound()
        {
            var taskId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns((TaskItem)null);
            var result = _controller.Delete(taskId);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}
