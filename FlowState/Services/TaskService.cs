using FlowState.Models;
using FlowState.Repositories;

namespace FlowState.Services
{
    public class TaskService
    {

        private readonly TaskRepo _taskRepo;

        public TaskService(TaskRepo repo)
        {
            _taskRepo = repo;
        }

        public List<ToDoTask> GetAllAlbums()
        {
            return _taskRepo.GetAllAlbums();
        }

    }
}
