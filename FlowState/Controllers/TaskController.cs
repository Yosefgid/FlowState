using FlowState.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowState.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;
        public TaskController(TaskService TaskService) 
        { 
            _taskService = TaskService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_taskService.GetAllAlbums());
        }
    }
}
