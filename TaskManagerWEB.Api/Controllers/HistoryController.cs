using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManagerWEB.Api.ViewModels;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HistoryController : Controller
    {

        private readonly IHistoryService _historyService;
        private readonly IMapper _mapper;

        public HistoryController( IHistoryService historyService,IMapper mapper)
        {
            _historyService = historyService;
            _mapper = mapper;
        }


        [HttpGet("getAll/{taskId}")]
        [ProducesResponseType(typeof(History), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll(int taskId)
        {
            var history = _historyService.GetAllByTaskId(taskId);
            var result = _mapper.Map<IEnumerable<History>,IEnumerable<HistoryVM>>(history);
            return Ok(result);
        }
    }
}
