using TodoApp_Backend.Models;

namespace TodoApp_Backend.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<Users?> IsUserExist(string username, string email, CancellationToken cancellationToken);
        Task<Users?> CheckUserByUsername(string username, CancellationToken cancellationToken);
        void AddUser(Users user);
        void RemoveUser(Users user);
        void AddRefreshToken(RefreshTokens refreshToken);
        void RemoveRefreshToken(RefreshTokens refreshToken);
        Task SaveUser(CancellationToken cancellationToken);
        Task<RefreshTokens?> IsTokenExist(string oldToken, CancellationToken cancellationToken);
    }
}
