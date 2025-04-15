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
    public class CommentServiceTest
    {
        private readonly CommentService _commentService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CommentService>> _loggerMock;
        private readonly Mock<IUserClaimService> _userClaimServiceMock;

        public CommentServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CommentService>>();
            _userClaimServiceMock = new Mock<IUserClaimService>();
            _commentService = new CommentService(_unitOfWorkMock.Object, _loggerMock.Object, _userClaimServiceMock.Object);
        }

        [Fact]
        public void GetAllByTaskId_ValidTaskId_ReturnsComments()
        {
            // Arrange
            int taskItemId = 1;
            var comments = new List<Comment>
            {
                new Comment { Id = 1, TaskItemId = taskItemId, Description = "Comment 1" },
                new Comment { Id = 2, TaskItemId = taskItemId, Description = "Comment 2" }
            };
            _unitOfWorkMock.Setup(uow => uow.Comment.GetAll(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>()))
                .Returns(comments);
            // Act
            var result = _commentService.GetAllByTaskId(taskItemId);
            // Assert
            Assert.Equal(comments, result);
            Assert.NotNull(result);
            Assert.IsType<List<Comment>>(result);
        }

        [Fact]
        public void GetAllByTaskId_NoCommentsFound_ThrowsException()
        {
            // Arrange
            int taskItemId = 1;
            var comments = new List<Comment>();
            _unitOfWorkMock.Setup(uow => uow.Comment.GetAll(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>()))
                .Returns(comments);
            // Act & Assert
            
            Assert.Throws<Exception>(() => _commentService.GetAllByTaskId(taskItemId));
        }


        [Fact]
        public void UpsertAsync_ValidComment_AddsComment()
        {
            // Arrange
            var comment = new Comment { Id = 0, TaskItemId = 1, Description = "New Comment"};
            var task = new TaskItem { Id = 1, Title = "Task 1" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(),It.IsAny<string>(),It.IsAny<bool>()))
                .Returns(task);
            _userClaimServiceMock.Setup(u => u.GetUserId()).Returns("userId");
            _unitOfWorkMock.Setup(uow => uow.Comment.Add(It.IsAny<Comment>()));
            _unitOfWorkMock.Setup(uow => uow.Save());
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                                .Returns(new AppUser());
            // Act
            var result =  _commentService.Upsert(comment);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(comment.Description, result.Description);
        }
        [Fact]
        public void UpsertAsync_CommentNotFound_ThrowsException()
        {
            // Arrange
            var comment = new Comment { Id = 1, TaskItemId = 1, Description = "Updated Comment" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new TaskItem { Id = 1, Title = "Task 1" });
            _userClaimServiceMock.Setup(u => u.GetUserId()).Returns("userId");
            _unitOfWorkMock.Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((Comment)null);
            // Act & Assert
            Assert.Throws<Exception>(() => _commentService.Upsert(comment));
        }
        [Fact]
        public void UpsertAsync_TaskNotFound_ThrowsException()
        {
            // Arrange
            var comment = new Comment { Id = 1, TaskItemId = 1, Description = "Updated Comment" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((TaskItem)null);
            // Act & Assert
            Assert.Throws<Exception>(() => _commentService.Upsert(comment));
        }
        [Fact]
        public void UpsertAsync_CommentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Comment comment = null;
            // Act & Assert
             Assert.Throws<ArgumentNullException>(() => _commentService.Upsert(comment));
        }
        [Fact]
        public void UpsertAsync_CommentIdIsZero_AddsNewComment()
        {
            // Arrange
            var comment = new Comment { Id = 0, TaskItemId = 1, Description = "New Comment" };
            var task = new TaskItem { Id = 1, Title = "Task 1" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(task);
            _userClaimServiceMock.Setup(u => u.GetUserId()).Returns("userId");
            _unitOfWorkMock.Setup(uow => uow.Comment.Add(It.IsAny<Comment>()));
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser());
            // Act
            var result = _commentService.Upsert(comment);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(comment.Description, result.Description);
        }
        [Fact]
        public void UpsertAsync_CommentIdIsNotZero_UpdatesExistingComment()
        {
            // Arrange
            var comment = new Comment { Id = 1, TaskItemId = 1, Description = "Updated Comment" };
            var task = new TaskItem { Id = 1, Title = "Task 1" };
            _unitOfWorkMock.Setup(uow => uow.TaskItem.Get(It.IsAny<Expression<Func<TaskItem, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(task);
            _userClaimServiceMock.Setup(u => u.GetUserId()).Returns("userId");
            _unitOfWorkMock.Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(comment);
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser());
            // Act
            var result =  _commentService.Upsert(comment);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(comment.Description, result.Description);
        }

        [Fact]
        public void Delete_ok()
        {
            _userClaimServiceMock
                .Setup(um => um.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_Admin)
                });
            _userClaimServiceMock
                .Setup(um => um.GetUser())
                .Returns(new AppUser { Id = "test-user-id" });
            var comment = new Comment { Id = 1, TaskItemId = 1, Description = "Test comment" };
            _unitOfWorkMock
                .Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(comment);
           var result = _commentService.Delete(1);
            Assert.NotNull(result);
            Assert.Equal(comment, result);
            _unitOfWorkMock.Verify(uow => uow.Comment.Remove(It.IsAny<Comment>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<Comment>(result);
            Assert.Equal(comment, result);
        }

        [Fact]
        public void Delete_unauthorized()
        {
            _userClaimServiceMock
                .Setup(um => um.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_User)
                });
            _userClaimServiceMock
                .Setup(um => um.GetUser())
                .Returns(new AppUser { Id = "test-user-id" });
            var comment = new Comment { Id = 1, TaskItemId = 1, Description = "Test comment" };
            _unitOfWorkMock
                .Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(comment);
            // Act & Assert
            Assert.Throws<Exception>(() => _commentService.Delete(1));
        }

        [Fact]
        public void Delete_commentNotFound()
        {
            _userClaimServiceMock
                .Setup(um => um.GetClaims()).Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, SD.Role_Admin)
                });
            _userClaimServiceMock
                .Setup(um => um.GetUser())
                .Returns(new AppUser { Id = "test-user-id" });
            var comment = new Comment { Id = 1, TaskItemId = 1, Description = "Test comment" };
            _unitOfWorkMock
                .Setup(uow => uow.Comment.Get(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((Comment)null);
            // Act & Assert
            Assert.Throws<Exception>(() => _commentService.Delete(1));
        }
    }
}
