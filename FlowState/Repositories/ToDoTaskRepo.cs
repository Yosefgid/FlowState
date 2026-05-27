using Microsoft.EntityFrameworkCore;
using FlowState.Models;
namespace FlowState.Repositories
{
    public interface IToDoTaskRepo
    {
        public List<ToDoTask> GetAllTasks();

        public ToDoTask? GetTask(int id);

        public ToDoTask AddTask(ToDoTask task);

        public ToDoTask? UpdateTask(int id, ToDoTask updatedTask);

        public bool DeleteTask(int id);

        public ToDoTask? ToggleTaskCompleted(int id);
    }
    public class ToDoTaskRepo : IToDoTaskRepo
    {
        private MyDbContext _dbContext;

        public ToDoTaskRepo(MyDbContext context)
        {
            _dbContext = context;   
        }

        // Basic CRUD --------------------------

        public List<ToDoTask> GetAllTasks()
        {
            return _dbContext.Tasks.ToList();
        }

        public ToDoTask? GetTask(int id)
        {

            var existingTask = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);

            if (existingTask == null)
                return null;

            return existingTask; 
        }


        public ToDoTask AddTask(ToDoTask task)
        {
            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();
            return task;
        }


        public ToDoTask? UpdateTask(int id, ToDoTask updatedTask)
        {
            var existingTask = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);

            if (existingTask == null)
                return null;

            _dbContext.Entry(existingTask).CurrentValues.SetValues(updatedTask);

            _dbContext.SaveChanges();

            return existingTask;
        }

        public bool DeleteTask(int id)
        {
            var task = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return false;

            _dbContext.Tasks.Remove(task);
            _dbContext.SaveChanges();

            return true;
        }

        // Additional Functionality --------------------------
        public ToDoTask? ToggleTaskCompleted(int id)
        {
            var task = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return null;

            task.TimeCompleted = task.IsCompleted ? task.TimeCompleted = null : task.TimeCompleted = DateTime.Now;

            task.IsCompleted = !task.IsCompleted;

            _dbContext.SaveChanges();
            return task;

        }


    }
}

