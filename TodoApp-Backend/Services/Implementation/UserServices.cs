using TodoApp_Backend.DTOs;
using TodoApp_Backend.Models;
using TodoApp_Backend.Repositories.Interface;
using TodoApp_Backend.Services.Interface;

namespace TodoApp_Backend.Services.Implementation
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _u;
        private readonly ICryptographyService _c;
        private readonly IJwtService _j;
        public UserServices(IUserRepository u, ICryptographyService c, IJwtService j) 
        {
            _u = u;
            _c = c;
            _j = j;
        }

        public async Task<UsersRegistrationResponse> Register (UsersRegistrationRequest request, CancellationToken cancellationToken)
        {
            var isUsernameExist = await _u.IsUserExist(request.Username, request.Email, cancellationToken);

            if (isUsernameExist != null)
            {
                if (isUsernameExist.Username.ToLower() == request.Username.ToLower())
                {
                    return new UsersRegistrationResponse { Message = "Username is Taken"};
                }
                else if (isUsernameExist.Email.ToLower() == request.Email.ToLower()) 
                {
                    return new UsersRegistrationResponse { Message = "Email is Taken" };
                }
            }
            else
            {
                var passwordHash = _c.HashPassword(request.Password);

                var newUser = new Users
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                };

                _u.AddUser(newUser);
                await _u.SaveUser(cancellationToken);
            }

            return new UsersRegistrationResponse
            {
                Message = "Success"
            };
        }

        public async Task<UsersLoginResponse> Login(UsersLoginRequest request, CancellationToken cancellationToken)
        {
            var checkUsernamel = await _u.CheckUserById(request.Username, cancellationToken);

            if (checkUsernamel == null || !_c.VerifyPassword(request.Password, checkUsernamel.PasswordHash))
            {
                return new UsersLoginResponse
                {
                    Message = "Email / Password is incorrect"
                };
            }
            else
            {
                var token = _j.GenerateToken(checkUsernamel);
                return new UsersLoginResponse
                {
                    Message = "Success",
                    Token = token
                };
            }
        }
    }
}
