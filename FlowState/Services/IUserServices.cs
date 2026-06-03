using FlowState.Models;

namespace FlowState.Services
{
    public interface IUserServices
    {
        List<User> GetAllUser();
        User? GetUserById(int id);
        bool DeleteUser(int id);
        User? UpdateUser(int id, User updated);
        User? ChangeUsername(int id, string newUsername);
    }
}
