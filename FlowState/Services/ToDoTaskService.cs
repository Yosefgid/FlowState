using FlowState.Models;
using FlowState.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Services
{
    public interface IToDoTaskService
    {
        public List<ToDoTask> GetAllTasks();


        public List<ToDoTask> GetAllTasksByUser(int userId);
        public ToDoTask? GetTask(int id);

        public ToDoTask AddTask(ToDoTask task);

        public ToDoTask? UpdateTask(int id, ToDoTask updatedTask);

        public bool DeleteTask(int id);

        public ToDoTask? ToggleTaskCompleted(int id);

        public void ToggleTasksCompleted(List<ToDoTask> tasks , bool IsDone);

        public void DeleteTasks(List<ToDoTask> tasks);

        public ToDoTask AssignEisen(int id, EisenCat cat);

        public List<ToDoTask> GetTasksBySession(int sessionId);

    }
    public class ToDoTaskService : IToDoTaskService
    {

        private readonly IToDoTaskRepo _taskRepo;

        public ToDoTaskService(IToDoTaskRepo repo)
        {
            _taskRepo = repo;
        }

        public List<ToDoTask> GetAllTasks()
        {
            return _taskRepo.GetAllTasks();
        }

        public List<ToDoTask> GetAllTasksByUser(int userId)        
        {
            return _taskRepo.GetAllTasksByUser(userId);
        }
        public ToDoTask? GetTask(int id)
        {

            return _taskRepo.GetTask(id);
        }


        public ToDoTask AddTask(ToDoTask task)
        {
            return _taskRepo.AddTask(task);
        }


        public ToDoTask? UpdateTask(int id, ToDoTask updatedTask)
        {
            return _taskRepo.UpdateTask(id,updatedTask);
        }

        public bool DeleteTask(int id)
        {
            return _taskRepo.DeleteTask(id);
        }

        public ToDoTask? ToggleTaskCompleted(int id)
        {
            return _taskRepo.ToggleTaskCompleted(id);
        }

        public void ToggleTasksCompleted(List<ToDoTask> tasks, bool IsDone)
        {
            _taskRepo.ToggleTasksCompleted(tasks, IsDone);
        }

        public void DeleteTasks(List<ToDoTask> tasks)
        {
            _taskRepo.DeleteTasks(tasks);
        }

        public ToDoTask AssignEisen(int id, EisenCat cat)
        {
            return _taskRepo.AssignEisen(id,cat);   
        }

        public List<ToDoTask> GetTasksBySession(int sessionId)
        {
            return _taskRepo.GetTasksBySession(sessionId);
        }


    }
}
