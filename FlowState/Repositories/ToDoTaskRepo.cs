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

        public void ToggleTasksCompleted(List<ToDoTask> tasks, bool IsDone);

        public void DeleteTasks(List<ToDoTask> tasks);

        public ToDoTask AssignEisen(int id, EisenCat cat);

        public List<ToDoTask> GetTasksBySession(int sessionId);
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

        public void DeleteTasks(List<ToDoTask> tasks)
        {

            foreach (ToDoTask t in tasks)
            {
                var task = _dbContext.Tasks.FirstOrDefault(x => x.Id == t.Id);

                if (task == null)
                    continue;

                _dbContext.Tasks.Remove(task);
            }

            _dbContext.SaveChanges();

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

        public void ToggleTasksCompleted(List<ToDoTask> tasks , bool IsDone )
        {

            foreach (ToDoTask t in tasks)
            {
                var task = _dbContext.Tasks.FirstOrDefault(x => x.Id == t.Id);

                if (task == null)
                    continue;

                task.TimeCompleted = IsDone ?  task.TimeCompleted = DateTime.Now : task.TimeCompleted = null;

                task.IsCompleted = IsDone;
            }

            _dbContext.SaveChanges();

        }

        public ToDoTask AssignEisen (int id , EisenCat cat)
        {
            var task = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return null;

            task.Category = cat;

            _dbContext.SaveChanges();
            return task;
        }

        public List<ToDoTask> GetTasksBySession(int sessionId)
        {
            var existingSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId);

            if (existingSession == null)
                return null;

            return _dbContext.Tasks.Where(y => y.SessionId == sessionId).ToList();
        }

        

    }
}

