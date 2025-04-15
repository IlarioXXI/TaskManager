using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.Services.ServicesInterfaces;

namespace TaskManager.Services.Services 
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CommentService> _logger;
        private readonly IUserClaimService _userClaimService;
        public CommentService(IUnitOfWork unitOfWork,ILogger<CommentService> logger,IUserClaimService userClaimService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userClaimService = userClaimService;
        }
        public IEnumerable<Comment> GetAllByTaskId(int taskItemId)
        {
            var comments = _unitOfWork.Comment.GetAll(c => c.TaskItemId == taskItemId);
            if (comments.IsNullOrEmpty())
            {
                throw new Exception("No comments found for this task");
            }
            return comments;
        }

        public Comment Upsert(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            var task = _unitOfWork.TaskItem.Get(t => t.Id == comment.TaskItemId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var userId = _userClaimService.GetUserId();
            if (comment.Id == 0)
            {
                comment.CreationDate = DateTime.Now;
                comment.AppUserId = userId;
                _unitOfWork.Comment.Add(comment);
                _unitOfWork.Save();
                comment.TaskItem = task;
                comment.AppUser = _unitOfWork.AppUser.Get(u=>u.Id == comment.AppUserId);
                _logger.LogInformation("User {email} added new comment to task: {task}", userId, task.Title);
                return comment;
            }
            else
            {
                var existingComment = _unitOfWork.Comment.Get(c => c.Id == comment.Id);
                if (existingComment == null)
                {
                    throw new Exception("Comment not found");
                }

                comment.AppUserId = existingComment.AppUserId;
                comment.TaskItemId = existingComment.TaskItemId;
                comment.CreationDate = DateTime.Now;
                _unitOfWork.Comment.Update(comment);
                _unitOfWork.Save();
                comment.TaskItem = task;
                comment.AppUser = _unitOfWork.AppUser.Get(u => u.Id == comment.AppUserId);
                _logger.LogInformation("User {email} changed comment {comment} for task: {task}", userId, existingComment.Description, task.Title);
                return comment;
            }
        }

        public Comment Delete(int id)
        {
            var userRole = _userClaimService.GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var user = _userClaimService.GetUser();
            if (userRole != SD.Role_Admin)
            {
                throw new Exception("User on authorized");
            }
            var comment = _unitOfWork.Comment.Get(x => x.Id == id);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }
            _unitOfWork.Comment.Remove(comment);
            _unitOfWork.Save();
            _logger.LogInformation("User {email} deleted comment : {comment}", user.Email, comment.Description);
            return comment;
        }
    }
}

