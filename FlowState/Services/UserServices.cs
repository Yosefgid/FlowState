using FlowState.Models;
using FlowState.Repositories;
using System.Security.Claims;


namespace FlowState.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepo _userRepo;

        public int UserId { get; set; }
        public UserServices(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public List<User> GetAllUser()
        {
            return _userRepo.GetAllUsers();
        }
        public User? GetUserById(int id)
        {
            return _userRepo.GetUserById(id);
        }
        public bool DeleteUser(int id)
        {
            return _userRepo.DeleteUser(id);
        }
        public User? UpdateUser(int id, User updated)
        {
            return _userRepo.UpdateUser(id, updated);
        }
        public User? ChangeUsername(int id, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername)) return null;
            var existing = _userRepo.GetAllUsers();
            bool usernameIsTaken = existing.Any(u => u.Username.ToLower() == newUsername.ToLower() && u.Id != id);
            if (usernameIsTaken) return null;
            return _userRepo.ChangeUsername(id, newUsername);
        }

        

    }
}
