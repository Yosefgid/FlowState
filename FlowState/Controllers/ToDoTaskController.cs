using FlowState.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowState.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class ToDoTaskController : Controller
    {
        private readonly ToDoTaskService _taskService;
        public ToDoTaskController(ToDoTaskService TaskService) 
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
