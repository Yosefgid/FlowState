using FlowState.Models;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class ToDoTaskController : AuthorizedControllerBase
    {
        private readonly IToDoTaskService _taskService;
        public ToDoTaskController(IToDoTaskService TaskService)
        {
            _taskService = TaskService;
        }
        [HttpGet("test/{id}")]
       

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            //return Ok(_taskService.GetAllTasks());
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            return Ok(_taskService.GetAllTasksByUser(userId.Value));
        }

        [HttpGet("session/{sessionId}")]
        public IActionResult GetAllTasksBySession(int sessionId)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            var tasks = _taskService.GetTasksBySession(sessionId) ?? new List<ToDoTask>();
            return Ok(tasks.Where(t=> t.UserId == userId.Value).ToList());

        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {

            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();


            var task = _taskService.GetTask(id);

            if (task == null || task.UserId != userId.Value) return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] ToDoTask task)
        {

            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();

            task.UserId = userId.Value; //this is maybe owner from the token not the body of the ToDoTask
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

            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();

            var existing = _taskService.GetTask(id);
            if (existing == null || existing.UserId != userId.Value) return NotFound();

            //Justification
            //This line is required becuase if this did not exist it would maybe overwrite the edit body 
            updatedTask.UserId = existing.UserId;

            var task = _taskService.UpdateTask(id, updatedTask);

            return Ok(task);
        }

     

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            var existing = _taskService.GetTask(id);
            if (existing == null || existing.UserId != userId.Value) return NotFound();
            _taskService.DeleteTask(id);

            return NoContent();
        }


        [HttpPatch("{id}/toggle")]
        public IActionResult ToggleTaskCompleted(int id)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            var existing = _taskService.GetTask(id);
            if (existing == null || existing.UserId != userId.Value) return NotFound();

            var updatedTask = _taskService.ToggleTaskCompleted(id);


            return Ok(updatedTask);
        }


        [HttpPatch("set-all/{IsDone}")]
        public IActionResult ToggleTasksCompleted(bool IsDone ,[FromBody] List<ToDoTask> tasks)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            _taskService.ToggleTasksCompleted(Owned(tasks, userId.Value),IsDone);
            return Ok();
        }

        [HttpPatch("delete-selected")]
        public IActionResult DeleteTasks([FromBody] List<ToDoTask> tasks)
        {

            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            _taskService.DeleteTasks(Owned(tasks, userId.Value));

            return Ok();
        }

        [HttpPatch("assign-eisen/{id}")]
        public IActionResult ChangeEisen(int id,[FromBody] EisenCat cat)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
           

            var existing = _taskService.GetTask(id);
            if (existing == null || existing.UserId != userId.Value) return NotFound();

            var updatedTask = _taskService.AssignEisen(id, cat);
            return Ok(updatedTask);
        }

        private List<ToDoTask> Owned(List<ToDoTask> tasks, int userId)
        {
            return tasks
                .Select(t => _taskService.GetTask(t.Id))
                .Where(t => t != null && t.UserId == userId)
                .Select(t => t!)
                .ToList();
        }

    }
}
