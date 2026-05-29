using FlowState.Models;

namespace FlowState.Services
{
    public interface IAuthServices
    {
        User? Register(string username, string email, string plainTextPassword);
        User? Login(string email, string plainTextPassword);
    }
}

