using TodoApp_Backend.Models;

namespace TodoApp_Backend.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<Users?> IsUserExist(string username, string email, CancellationToken cancellationToken);
        Task<Users?> CheckUserByUsername(string username, CancellationToken cancellationToken);
        void AddUser(Users user);
        void RemoveUser(Users user);
        Task SaveUser(CancellationToken cancellationToken);
    }
}
