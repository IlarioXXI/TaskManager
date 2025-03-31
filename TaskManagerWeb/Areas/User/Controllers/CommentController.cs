using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories;
using TaskManager.DataAccess.Repositories.Interfaces;

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

        public IActionResult Upsert(int taskId,int? id)
        {
            if (taskId!= 0 || taskId != null)
            {
                var taskItem = _unitOfWork.TaskItem.Get(t => t.Id == taskId);
                Comment comment = new()
                {
                    AppUser = _unitOfWork.AppUser.Get(u=>u.Id == taskItem.AppUserId),
                    TaskItemId = taskId
                };
                return View(comment);
            }
            else
            {
                var commentToUpdate = _unitOfWork.Comment.Get(c=>c.Id == id);
                return View(commentToUpdate);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Comment comment)
        {
            if (_unitOfWork.TaskItem.Get(t => t.Id == comment.TaskItemId).AppUserId != null)
            {
                comment.AppUserId = _unitOfWork.TaskItem.Get(t => t.Id == comment.TaskItemId).AppUserId;
            }
            comment.CreationDate = DateTime.Now;
            _unitOfWork.Comment.Add(comment);
            _unitOfWork.Save();
            return RedirectToAction("Index","Team");
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
