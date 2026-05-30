using FlowState.Models;
using FlowState.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Services
{
    public interface IToDoTaskService
    {
        public List<ToDoTask> GetAllTasks();

        public ToDoTask? GetTask(int id);

        public ToDoTask AddTask(ToDoTask task);

        public ToDoTask? UpdateTask(int id, ToDoTask updatedTask);

        public bool DeleteTask(int id);

        public ToDoTask? ToggleTaskCompleted(int id);

        public void ToggleTasksCompleted(List<ToDoTask> tasks , bool IsDone);

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

    

    }
}
