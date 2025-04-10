using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Extensions;
using TaskManagerWeb.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskItemController : Controller
    {


        private readonly ILogger<TaskItemController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ToDoVM> _validator;
        public TaskItemController(ILogger<TaskItemController> logger, IUnitOfWork unitOfWork, IValidator<ToDoVM> validation)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validator = validation;
        }



        // GET: api/<TaskItemController>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(TaskItem),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            else
            {
                var allTasks = _unitOfWork.TaskItem.GetAll(t => t.AppUserId == userId);
                return Ok(allTasks);
            }

        }

        // GET api/<TaskItemController>/
        [HttpGet("getById/{id}")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetById(int id)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            else
            {
                var task = _unitOfWork.TaskItem.Get(t => t.Id == id);
                return Ok(task);
            }
        }

        // POST api/<TaskItemController>
        [HttpPost("upsert/{teamId}")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upsert(int teamId, [FromBody] ToDoVM taskItem)
        {
            var resultValidation = await _validator.ValidateAsync(taskItem);
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

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);


            if (taskItem.TaskToDo.Id == 0)
            {
                taskItem.TaskToDo.TeamId = teamId;
                _unitOfWork.TaskItem.Add(taskItem.TaskToDo);
                _unitOfWork.Save();
                _logger.LogInformation("TaskItem ({taskItemName}) created successfully by : {email}",taskItem.TaskToDo.Title,user.Email);
                return Ok(taskItem.TaskToDo);
            }
            else
            {
                var existingItem = _unitOfWork.TaskItem.Get(t=>t.Id == taskItem.TaskToDo.Id);
                var oldStatus = _unitOfWork.Status.Get(s=>s.Id == existingItem.StatusId);
                var oldPriority = _unitOfWork.Priority.Get(p=>p.Id == existingItem.PriorityId);
                existingItem.StatusId = taskItem.TaskToDo.StatusId;
                existingItem.PriorityId = taskItem.TaskToDo.PriorityId;
                if (oldStatus.Id != existingItem.StatusId || oldPriority.Id != existingItem.PriorityId)
                {
                    History history = new()
                    {
                        FromStatus = oldStatus.Name,
                        ToStatus = _unitOfWork.Status.Get(s => s.Id == existingItem.StatusId).Name,
                        ChangeDate = DateTime.Now,
                        TaskItemId = existingItem.Id,
                        AppUserId = userId,
                    };
                    _unitOfWork.History.Add(history);
                    _unitOfWork.Save();
                }
                if (existingItem.AppUserId != null)
                {
                    existingItem.AppUserId = taskItem.TaskToDo.AppUserId;
                }
                existingItem.Description = taskItem.TaskToDo.Description;
                existingItem.DueDate = taskItem.TaskToDo.DueDate;

                _unitOfWork.TaskItem.Update(existingItem);
                _unitOfWork.Save();
                _logger.LogInformation("TaskItem ({taskItemName}) updated successfully by : {email}", taskItem.TaskToDo.Title, user.Email);
                return Ok(existingItem  );
            }  
        }

        // DELETE api/<TaskItemController>/5
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (User.IsInRole(SD.Role_User))
            {
                return Unauthorized();
            }
            if (_unitOfWork.TaskItem.Get(x=>x.Id == id) == null)
            {
                return NotFound();
            }
            _unitOfWork.TaskItem.Remove(_unitOfWork.TaskItem.Get(x => x.Id == id));
            _unitOfWork.Save();
            _logger.LogInformation("TaskItem ({taskItemId}) deleted successfully by : {email}", id,user.Email);
            return Ok("Delete successfully");            
        }
    }
}
