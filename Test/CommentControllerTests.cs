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
using TaskManager.Services.Services;
using TaskManagerWEB.Api.Controllers;
using Xunit;

namespace Test
{
    public class CommentControllerTests
    {
        private readonly CommentController _controller;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IValidator<Comment>> _validatorMock;
        private readonly Mock<ILogger<CommentController>> _loggerMock;
      

        public CommentControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<Comment>>();
            _loggerMock = new Mock<ILogger<CommentController>>();
            _controller = new CommentController(_loggerMock.Object, _unitOfWorkMock.Object, _validatorMock.Object);
        }
        [Fact]
        public async void GetAllByTaskId_ok()
        {
            int taskItemId = 1;
            var comments = new List<Comment>
            {
                new Comment { Id = 1, TaskItemId = taskItemId, Description = "Comment 1" },
                new Comment { Id = 2, TaskItemId = taskItemId, Description = "Comment 2" }
            };
            _unitOfWorkMock.Setup(uow => uow.Comment.GetAll(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>())).Returns(comments);

            var jsonResult = _controller.GetAllByTaskId(taskItemId);
            var okResult = Assert.IsType<OkObjectResult>(jsonResult);
            Assert.Equal(okResult.Value, comments);
        }
        [Fact]
        public async void GetAllByTaskId_notFound()
        {
            int taskItemId = 1;
            var comments = new List<Comment>();
            _unitOfWorkMock.Setup(uow => uow.Comment.GetAll(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>()))
                .Returns(comments);
            var jsonResult = _controller.GetAllByTaskId(taskItemId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(jsonResult);
        }

        [Fact]
        public async void UpsertAsync_ValidComment_ReturnsOk()
        {
            var comment = new Comment { Id = 1, Description = "Test comment", TaskItemId = 1 };



            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                }))
            };

            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name});



            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new TaskItem { Id = 1, Title = "Test task" });
            _unitOfWorkMock.Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Comment());



            var result = await _controller.UpsertAsync(comment);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(comment, okResult.Value);
        }

        [Fact]
        public async void UpsertAsync_ValidComment_ReturnsOk_update()
        {
            var comment = new Comment { Id = 1, Description = "Test comment", TaskItemId = 1 };
            var commentToUpdate = new Comment { Id = 1, Description = "Test comment to update", TaskItemId = 1 };


            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                }))
            };

            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });


            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new TaskItem { Id = 1, Title = "Test task" });
            _unitOfWorkMock.Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(commentToUpdate);

            var result = await _controller.UpsertAsync(comment);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(comment, okResult.Value);
        }

        [Fact]
        public async void UpsertAsync_InvalidComment_ReturnsBadRequest()
        {
            var comment = new Comment { Id = 1, Description = "Test comment", TaskItemId = 1 };
            var validationResult = new ValidationResult
            {
                Errors = new List<ValidationFailure>
                {
                    new ValidationFailure("Description", "Description is required")
                }
            };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var result = await _controller.UpsertAsync(comment);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async void DeleteAsync_ValidComment_ReturnsOk()
        {
            var comment = new Comment { Id = 1, Description = "Test comment", TaskItemId = 1 };

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-admin-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };

            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });

            _unitOfWorkMock.Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(comment);
            var result = await _controller.DeleteAsync(comment.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(comment, okResult.Value);
        }

        [Fact]
        public async void DeleteAsync_CommentNotFound_ReturnsNotFound()
        {
            var commentId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-admin-id"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            _unitOfWorkMock.Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((Comment)null);
            var result = await _controller.DeleteAsync(commentId);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void DeleteAsync_Unauthorized()
        {
            var commentId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-admin-id"),
                    new Claim(ClaimTypes.Role, "User")
                }))
            };
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser { Id = _controller.HttpContext.User.Identity.Name });
            var result = await _controller.DeleteAsync(commentId);
            var notFoundResult = Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
