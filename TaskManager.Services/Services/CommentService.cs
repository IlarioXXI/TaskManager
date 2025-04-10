using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Repositories;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Services.ServicesInterfaces;

namespace TaskManager.Services.Services 
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CommentService> _logger;
        private readonly IValidator<Comment> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommentService(IUnitOfWork unitOfWork,ILogger<CommentService> logger, IValidator<Comment> validator,IHttpContextAccessor httpCon)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
            _httpContextAccessor = httpCon;
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

        public async Task<Comment> UpsertAsync(Comment comment)
        {
            var resultValidation = await _validator.ValidateAsync(comment);
            if (!resultValidation.IsValid)
            {
                throw new ValidationException(resultValidation.Errors);
            }

            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            var task = _unitOfWork.TaskItem.Get(t => t.Id == comment.TaskItemId);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.Id == 0)
            {
                comment.CreationDate = DateTime.Now;
                comment.AppUserId = userId;
                _unitOfWork.Comment.Add(comment);
                _unitOfWork.Save();
                _logger.LogInformation("User {email} added new comment to task: {task}", userId, task.Title);
            }
            else
            {
                var existingComment = _unitOfWork.Comment.Get(c => c.Id == comment.Id);
                if (existingComment == null)
                {
                    throw new Exception("Comment not found");
                }

                existingComment.AppUserId = userId;
                existingComment.TaskItemId = comment.TaskItemId;
                existingComment.Description = comment.Description;
                existingComment.CreationDate = DateTime.Now;
                _unitOfWork.Comment.Update(existingComment);
                _unitOfWork.Save();
                _logger.LogInformation("User {email} changed comment {comment} for task: {task}", userId, existingComment.Description, task.Title);
            }

            return comment;
        }

        public Comment Delete(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (!_httpContextAccessor.HttpContext.User.IsInRole(SD.Role_Admin))
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

