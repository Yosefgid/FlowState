using Microsoft.EntityFrameworkCore;
using FlowState.Models;
namespace FlowState.Repositories
{
    public class TaskRepo
    {
        private MyDbContext _dbContext;

        public TaskRepo(MyDbContext context)
        {
            _dbContext = context;   
        }

        public List<ToDoTask> GetAllAlbums()
        {
            return _dbContext.Tasks.ToList();
        }
    }
}
