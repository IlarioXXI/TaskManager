using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Comment> _validator;
        

        public CommentController(ILogger<CommentController> logger, IUnitOfWork unitOfWork,IValidator<Comment> validator)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validator = validator;

        }

        [HttpGet("GetAllByTaskId/{taskItemId}")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllByTaskId(int taskItemId)
        {
            var comments = _unitOfWork.Comment.GetAll(c => c.TaskItemId == taskItemId);
            if (comments.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(comments);
        }


        [HttpPost("Upsert")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertAsync(Comment comment)
        {
            var resultValidation = await _validator.ValidateAsync(comment);
            if (!resultValidation.IsValid)
            {
                resultValidation.AddToModelState(this.ModelState);
                var errors = new List<string>();
                foreach (var error in resultValidation.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
                return BadRequest(errors);
            }
            if (comment == null)
            {
                return BadRequest();
            }
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u=>u.Id == userId);
            
            var task = _unitOfWork.TaskItem.Get(t => t.Id == comment.TaskItemId);
            if (task == null)
            {
                return NotFound();
            }
            if (comment.Id == 0 || comment.Id == null)
            {

                comment.CreationDate = DateTime.Now;
                comment.AppUserId = userId;
                _unitOfWork.Comment.Add(comment);
                _unitOfWork.Save();
                _logger.LogInformation("User {email} added new comment ad task : {task}",user.Email,task.Title);
                return Ok(comment);
            }
            else
            {
                var existingComment = _unitOfWork.Comment.Get(c=>c.Id == comment.Id);
                existingComment.AppUserId = userId;
                existingComment.TaskItemId = comment.TaskItemId;
                existingComment.Description = comment.Description;
                existingComment.CreationDate = DateTime.Now;
                _unitOfWork.Comment.Update(existingComment);
                _unitOfWork.Save();
                _logger.LogInformation("User {email} changed {comment} comment ad task : {task}", user.Email, existingComment.Description,task.Title);
            }
            return Ok(comment);
        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            var comment = _unitOfWork.Comment.Get(x=>x.Id == id);
            if (comment==null)
            {
                return NotFound();
            }
            _unitOfWork.Comment.Remove(comment);
            _unitOfWork.Save();
            _logger.LogInformation("User {email} deleted comment : {comment}", user.Email, comment.Description);
            return Ok(comment);
        }
    }
}
