using Microsoft.EntityFrameworkCore;
using FlowState.Models;
namespace FlowState.Repositories
{
    public class ToDoTaskRepo
    {
        private MyDbContext _dbContext;

        public ToDoTaskRepo(MyDbContext context)
        {
            _dbContext = context;   
        }

        public List<ToDoTask> GetAllAlbums()
        {
            return _dbContext.Tasks.ToList();
        }
    }
}
