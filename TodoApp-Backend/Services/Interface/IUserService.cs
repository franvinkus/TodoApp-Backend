using TodoApp_Backend.DTOs;

namespace TodoApp_Backend.Services.Interface
{
    public interface IUserService
    {
        Task<UsersRegistrationResponse> Register(UsersRegistrationRequest request, CancellationToken cancellationToken);
        Task<UsersLoginResponse> Login(UsersLoginRequest request, CancellationToken cancellationToken);
        Task<bool> Logout(string refreshToken, CancellationToken cancellationToken);
        Task<UsersLoginResponse> RefreshToken(string oldToken, CancellationToken cancellationToken); 
    }
}
