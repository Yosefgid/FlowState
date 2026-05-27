using FlowState.Models;

namespace FlowState.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly MyDbContext _context;
        public UserRepo(MyDbContext context)
        {
            _context = context;
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();

        }
        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public User? AddUser(User user)
        {
            if (user == null) return null;
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public bool DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;


        }
        public User? UpdateUser(int id, User updatedUser)
        {
            var currUser = GetUserById(id);
            if (currUser == null) return null;
            currUser.Username = updatedUser.Username;
            currUser.Email = updatedUser.Email;
            currUser.PasswordHash = updatedUser.PasswordHash;
            _context.SaveChanges();
            return currUser;

        }
        public User? ChangeUsername(int id, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername)) return null;
            var user = GetUserById(id);
            if (user == null) return null;
            //hard coding to make sure names of the samekind but differnt Cases are treated the same
            //Test fails
            bool isTaken = _context.Users.Any(u => u.Username.ToLower() == newUsername.ToLower() && u.Id != id);
            if (isTaken) return null;
            //we could throw an excpetion but then we would have to do a try catch later on 
            user.Username = newUsername;
            _context.SaveChanges();
            return user;
        }
    }
}
