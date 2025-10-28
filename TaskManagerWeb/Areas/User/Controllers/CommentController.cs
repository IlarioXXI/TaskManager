using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;

namespace TaskManagerWeb.Areas.User.Controllers
{
    [Area("User")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int taskId)
        {
            var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == taskId);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Comment comment = new()
            {
                AppUser = _unitOfWork.AppUser.Get(u=>u.Id == userId),
                TaskItemId = taskItem.Id
            }; 
            comment.AppUserId = userId;
            return View(comment);
        }

        [HttpPost]
        public IActionResult Upsert(Comment comment, string? returnUrl)
        {
            comment.CreationDate = DateTime.Now;
            _unitOfWork.Comment.Add(comment);
            _unitOfWork.Save();
            
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Details", "TaskItem", new { id = comment.TaskItemId });
        }

        public IActionResult Delete(int id)
        {
            var commentToDelete = _unitOfWork.Comment.Get(c => c.Id == id);
            _unitOfWork.Comment.Remove(commentToDelete);
            _unitOfWork.Save();
            return RedirectToAction("Details","TaskItem", new { id = commentToDelete.TaskItemId });
        }
    }
}
