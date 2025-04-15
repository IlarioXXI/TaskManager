using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Extensions;
using TaskManager.Services.Services;
using TaskManager.Services.ServicesInterfaces;
using TaskManagerWeb.Api.Models;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IValidator<CommentVM> _validator;
        private readonly ICommentService _commentService;


        public CommentController(IValidator<CommentVM> validator,ICommentService commentService,IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
            _validator = validator;
        }



        [HttpGet("GetAllByTaskId/{taskItemId}")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllByTaskId(int taskItemId)
        {
            var comments = _commentService.GetAllByTaskId(taskItemId);
            var result = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentVM>>(comments);
            return Ok(result);
        }

        [HttpPost("Upsert")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertAsync(CommentVM commentVM)
        {
            var resultValidation = await _validator.ValidateAsync(commentVM);
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
            Comment comment = _mapper.Map<CommentVM,Comment>(commentVM);
            var commentUpdatedOrCreated = _commentService.Upsert(comment);
            var result = _mapper.Map<Comment, CommentVM>(commentUpdatedOrCreated);
            return Ok(result);
        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var commentDeleted = _commentService.Delete(id);
            var result = _mapper.Map<Comment, CommentVM>(commentDeleted);
            return Ok(result);
        }
    }
}
