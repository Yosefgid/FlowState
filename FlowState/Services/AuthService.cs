using FlowState.Models;
using FlowState.Repositories;

namespace FlowState.Services
{
    public class AuthService : IAuthServices
    {
        private readonly IUserRepo _userRepo;
        public AuthService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
       public User? Register(string username, string email, string plainTextPassword)
        {
            //if a an email is already in a db return null
           if(_userRepo.GetUserByEmail(email.ToLower()) !=null) return null;
            if (_userRepo.GetUserByUsername(username.ToLower()) != null) return null;

            var user = new User
            {
                Username = username.ToLower(),
                Email = email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword),
                CreatedAt = DateTime.UtcNow
            };
            return _userRepo.AddUser(user);
        }
       public User? Login(string email, string plainTextPassword)
        {
            var user = _userRepo.GetUserByEmail(email.ToLower());
            if (user == null) return null;
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, user.PasswordHash) ? user : null;
        }
    }
}
