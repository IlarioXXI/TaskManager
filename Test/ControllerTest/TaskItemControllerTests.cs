using AutoMapper;
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
using TaskManager.Services.Interfaces;
using TaskManager.Services.Services;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.Controllers;
using TaskManagerWEB.Api.ViewModels;
using Xunit;

namespace Test.ControllerTest
{
    public class TaskItemControllerTests
    {
        private readonly Mock<IValidator<TaskItemVM>> _validatorMock;
        private readonly Mock<ITaskItemService> _taskItemServiceMock;
        private readonly Mock<IMapper> _mapper;
        private readonly TaskItemController _controller;

        public TaskItemControllerTests()
        {
            _validatorMock = new Mock<IValidator<TaskItemVM>>();
            _taskItemServiceMock = new Mock<ITaskItemService>();
            _mapper = new Mock<IMapper>();
            _controller = new TaskItemController(_validatorMock.Object, _taskItemServiceMock.Object, _mapper.Object);
        }

        [Fact]
        public void GetAll_Ok()
        {
            var taskItems = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Test task 1" },
                new TaskItem { Id = 2, Title = "Test task 2" }
            };
            var taskItemsVM = new List<TaskItemVM>
            {
                new TaskItemVM { Id = 1, Title = "Test task 1" },
                new TaskItemVM { Id = 2, Title = "Test task 2" }
            };
            _taskItemServiceMock.Setup(s => s.GetAll())
                .Returns(taskItems);
            _mapper
                .Setup(s => s.Map<IEnumerable<TaskItem>, IEnumerable<TaskItemVM>>(taskItems))
                .Returns(taskItemsVM);
            var result = _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<TaskItemVM>>(okResult.Value);
            Assert.Equal(taskItemsVM, returnValue);
        }


        [Fact]
        public void GetById_OK()
        {
            var taskItem = new TaskItem
            {
                Id = 1,
                Title = "Test task",
                Description = "Test description",
                DueDate = DateTime.Now.AddDays(7),
                StatusId = 1,
                PriorityId = 1,
                AppUserId = "AppUser-UserValidation-id",
                TeamId = 1
            };
            var taskItemVm = new TaskItemVM
            {
                Id = 1,
                Title = "Test task",
                Description = "Test description",
                DueDate = DateTime.Now.AddDays(7),
                StatusId = 1,
                PriorityId = 1,
                AppUserId = "AppUser-UserValidation-id",
                TeamId = 1
            };
            _taskItemServiceMock.Setup(s => s.GetById(It.IsAny<int>()))
                .Returns(taskItem);
            _mapper
                .Setup(s => s.Map<TaskItem, TaskItemVM>(taskItem))
                .Returns(taskItemVm);
            var result = _controller.GetById(taskItem.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TaskItemVM>(okResult.Value);
            Assert.Equal(taskItemVm, returnValue);

        }


        [Fact]
        public async Task Upsert_ValidTaskItem_ReturnsOk_Add()
        {
            var taskItemVM = new TaskItemVM
            {
                Title = "Test task",
                Description = "Test description",
                DueDate = DateTime.Now.AddDays(7),
                StatusId = 1,
                PriorityId = 1,
                AppUserId = "AppUser-UserValidation-id",
                TeamId = 1
            };
            var taskItem = new TaskItem
            {
                Title = "Test task",
                Description = "Test description",
                DueDate = taskItemVM.DueDate,
                StatusId = 1,
                PriorityId = 1,
                AppUserId = "AppUser-UserValidation-id",
                TeamId = 1
            };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<TaskItemVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _mapper
                .Setup(s => s.Map<TaskItemVM, TaskItem>(taskItemVM))
                .Returns(taskItem);
            _taskItemServiceMock
                .Setup(s => s.Upsert(It.IsAny<TaskItem>()))
                .Returns(taskItem);
            _mapper
                .Setup(s => s.Map<TaskItem, TaskItemVM>(taskItem))
                .Returns(taskItemVM);
            var result = await _controller.Upsert(taskItemVM);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TaskItemVM>(okResult.Value);
            Assert.Equal(taskItemVM, returnValue);

        }


        [Fact]
        public async Task Upsert_ValidTaskItem_BadRequest()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<TaskItemVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure> { new ValidationFailure("Title", "Title is required") } });
            var result = await _controller.Upsert(new TaskItemVM());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

       

        [Fact]
        public void Delete_Ok()
        {
            var taskItem = new TaskItem
            {
                Id = 1,
                Title = "Test task",
                Description = "Test description",
                DueDate = DateTime.Now.AddDays(7),
                StatusId = 1,
                PriorityId = 1,
                AppUserId = "AppUser-UserValidation-id",
                TeamId = 1
            };
            _taskItemServiceMock.Setup(s => s.Delete(It.IsAny<int>()))
                .Returns(true);
            var result = _controller.Delete(taskItem.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<bool>(okResult.Value);
            Assert.True(returnValue);
        }

    }
}
