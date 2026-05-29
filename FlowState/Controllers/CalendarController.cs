using FlowState.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowState.Controllers
{
    [Route("api/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IToDoTaskService _taskService;

        public CalendarController(IToDoTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok(_taskService.GetAllTasks());
        }
    }
}