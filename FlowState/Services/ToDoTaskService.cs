using FlowState.Models;
using FlowState.Repositories;

namespace FlowState.Services
{
    public class ToDoTaskService
    {

        private readonly ToDoTaskRepo _taskRepo;

        public ToDoTaskService(ToDoTaskRepo repo)
        {
            _taskRepo = repo;
        }

        public List<ToDoTask> GetAllAlbums()
        {
            return _taskRepo.GetAllAlbums();
        }

    }
}
