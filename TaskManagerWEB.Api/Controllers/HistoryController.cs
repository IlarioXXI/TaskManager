using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HistoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public HistoryController( IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }


        [HttpGet("getAll/{taskId}")]
        [ProducesResponseType(typeof(History), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll(int taskId)
        {
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            }
            else
            {
                var historyTask = _unitOfWork.History.GetAll(h => h.TaskItemId == taskId);
                return Ok(historyTask );
            }

        }
    }
}
