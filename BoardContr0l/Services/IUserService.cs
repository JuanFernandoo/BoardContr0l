using BoardContr0l.Models;

namespace BoardContr0l.Services
{
    public interface IUserService
    {
        Task<User> RegisterUser(string username, string password, string email, int roleId);
        Task<User> Authenticate(string username, string password);
    }
}
