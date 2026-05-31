using FlowState.Models;

namespace FlowState.Repositories
{
    public interface IUserRepo
    {
        List<User> GetAllUsers();
        User? GetUserById(int id);
        User? AddUser(User user);
        bool DeleteUser(int id);
        User? UpdateUser(int id, User user);
        User? ChangeUsername(int id, string newUsername);
        User? GetUserByEmail(string email);
        User? GetUserByUsername(string username);
    }
}
