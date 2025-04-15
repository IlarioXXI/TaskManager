using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Extensions;
using TaskManager.Services.Interfaces;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskItemController : Controller
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<TaskItemVM> _validator;
        private readonly IMapper _mapper;
        public TaskItemController(IValidator<TaskItemVM> validation,ITaskItemService taskItemService,IMapper mapper)
        {
            _taskItemService = taskItemService;
            _validator = validation;
            _mapper = mapper;
        }



        // GET: api/<TaskItemController>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(TaskItem),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
        {
            var taskItems = _taskItemService.GetAll();
            var result = _mapper.Map<IEnumerable<TaskItem>,IEnumerable<TaskItemVM>>(taskItems);
            return Ok(result);
        }

        // GET api/<TaskItemController>/
        [HttpGet("getById/{id}")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetById(int id)
        {
            var taskItem = _taskItemService.GetById(id);
            var result = _mapper.Map<TaskItem, TaskItemVM>(taskItem);
            return Ok(result);
        }

        // POST api/<TaskItemController>
        [HttpPost("upsert")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upsert([FromBody] TaskItemVM taskItem)
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
            var taskItemToUpdateOrCreate = _mapper.Map<TaskItemVM, TaskItem>(taskItem);
            var taskItemResult = _taskItemService.Upsert(taskItemToUpdateOrCreate);
            var result = _mapper.Map<TaskItem, TaskItemVM>(taskItemResult);
            return Ok(result);
        }

        // DELETE api/<TaskItemController>/5
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return Ok(_taskItemService.Delete(id));
        }
    }
}
