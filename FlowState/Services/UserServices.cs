using FlowState.Models;
using FlowState.Repositories;

namespace FlowState.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepo _userRepo;
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
        public User? AddUser(User user, string passwordTxt)
        {
            var existing = _userRepo.GetAllUsers();
            bool usernameIsTaken = existing.Any(u => u.Username.ToLower() == user.Username.ToLower());
            if (usernameIsTaken) return null;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordTxt);
            return _userRepo.AddUser(user);

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
