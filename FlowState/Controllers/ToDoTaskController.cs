using FlowState.Models;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlowState.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class ToDoTaskController : Controller
    {
        private readonly IToDoTaskService _taskService;
        public ToDoTaskController(IToDoTaskService TaskService) 
        { 
            _taskService = TaskService;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok(_taskService.GetAllTasks());
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _taskService.GetTask(id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] ToDoTask task)
        {
            var createdTask = _taskService.AddTask(task);

            return CreatedAtAction(
                nameof(GetTask),
                new { id = createdTask.Id },
                createdTask
            );
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] ToDoTask updatedTask)
        {
            var task = _taskService.UpdateTask(id, updatedTask);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

     

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var deleted = _taskService.DeleteTask(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }


        [HttpPatch("{id}/toggle")]
        public IActionResult ToggleTaskCompleted(int id)
        {

            var updatedTask = _taskService.ToggleTaskCompleted(id);

            if (updatedTask == null)
                return NotFound();


            return Ok(updatedTask);
        }
    }
}
