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
using TaskManager.Services.Services;
using TaskManager.Services.ServicesInterfaces;
using TaskManagerWeb.Api.Models;
using TaskManagerWEB.Api.Controllers;
using Xunit;

namespace Test.ControllerTest
{
    public class CommentControllerTests
    {
        private readonly CommentController _controller;
        private readonly Mock<IValidator<CommentVM>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICommentService> _commentService;


        public CommentControllerTests()
        {
            _validatorMock = new Mock<IValidator<CommentVM>>();
            _mapperMock = new Mock<IMapper>();
            _commentService = new Mock<ICommentService>();

            _controller = new CommentController(_validatorMock.Object,_commentService.Object,_mapperMock.Object);
        }
        [Fact]
        public void GetAllByTaskId_ok()
        {
            int taskId = 1;
            var comments = new List<Comment>
            {
                new Comment { Id = 1, Description = "Test comment 1", TaskItemId = 1 },
                new Comment { Id = 2, Description = "Test comment 2", TaskItemId = 1 }
            };
            _commentService.Setup(c => c.GetAllByTaskId(It.IsAny<int>()))
                .Returns(comments);
            _mapperMock.Setup(m => m.Map<IEnumerable<Comment>,IEnumerable<CommentVM>>(It.IsAny<List<Comment>>()))
                .Returns(new List<CommentVM>
                {
                    new CommentVM { Id = 1, Description = "Test comment 1", TaskItemId = 1 },
                    new CommentVM { Id = 2, Description = "Test comment 2", TaskItemId = 1 }
                });
            var result = _controller.GetAllByTaskId(taskId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CommentVM>>(okResult.Value);
        }

        [Fact]
        public async Task UpsertAsync_ValidComment_ReturnsOk()
        {
            var commentVM = new CommentVM { Id = 0, Description = "Test comment", TaskItemId = 1 };
            Comment comment = new Comment { Id = 0, Description = "Test comment", TaskItemId = 1 };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _mapperMock.Setup(m => m.Map<CommentVM,Comment>(commentVM)).Returns(comment);
            _commentService.Setup(c => c.Upsert(It.IsAny<Comment>()))
                .Returns(comment);
            _mapperMock.Setup(m => m.Map<Comment, CommentVM>(comment)).Returns(commentVM);
            var result = await _controller.UpsertAsync(commentVM);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CommentVM>(okResult.Value);
            Assert.Equal(comment.Id, returnValue.Id);
        }

        [Fact]
        public async void UpsertAsync_ValidComment_ReturnsOk_update()
        {
            var commentVM = new CommentVM { Id = 0, Description = "Test comment", TaskItemId = 1 };
            Comment comment = new Comment { Id = 0, Description = "Test comment", TaskItemId = 1 };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _mapperMock.Setup(m => m.Map<CommentVM, Comment>(commentVM)).Returns(comment);
            _commentService.Setup(c => c.Upsert(It.IsAny<Comment>()))
                .Returns(comment);
            _mapperMock.Setup(m => m.Map<Comment, CommentVM>(comment)).Returns(commentVM);
            var result = await _controller.UpsertAsync(commentVM);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CommentVM>(okResult.Value);
            Assert.Equal(comment.Id, returnValue.Id);
        }

        [Fact]
        public async void UpsertAsync_InvalidComment_ReturnsBadRequest()
        {
            var commentVM = new CommentVM { Id = 1, Description = "Test comment", TaskItemId = 1 };
            var validationResult = new ValidationResult
            {
                Errors = new List<ValidationFailure>
                {
                    new ValidationFailure("Description", "Description is required")
                }
            };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentVM>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var result = await _controller.UpsertAsync(commentVM);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async void DeleteAsync_ValidComment_ReturnsOk()
        {
            var comment = new Comment { Id = 1, Description = "Test comment", TaskItemId = 1 };
            _commentService.Setup(c => c.Delete(It.IsAny<int>()))
                .Returns(comment);
            _mapperMock.Setup(m => m.Map<Comment, CommentVM>(comment)).Returns(new CommentVM { Id = 1, Description = "Test comment", TaskItemId = 1 });
            var result = _controller.Delete(1);
            Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CommentVM>(okResult.Value);
            Assert.Equal(comment.Id, returnValue.Id);
        }

    }
}
