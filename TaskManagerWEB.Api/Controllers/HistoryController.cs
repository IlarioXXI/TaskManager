using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManagerWeb.Models;

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
