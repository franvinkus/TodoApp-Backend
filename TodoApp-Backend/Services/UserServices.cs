using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TodoApp_Backend.Data;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Models;

namespace TodoApp_Backend.Services.Interface
{
    public class UserServices
    {
        private readonly TodoAppDbContext _db;
        private readonly IConfiguration _configuration;
        public UserServices(TodoAppDbContext db, IConfiguration configuration) 
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<UsersRegistrationResponse> Register (UsersRegistrationRequest request, CancellationToken cancellationToken)
        {
            var isUsernameExist = await _db.Users
                .Where(x => x.Username == request.Username || x.Email.ToLower() == request.Email.ToLower())
                .Select(x => new { x.Username, x.Email})
                .FirstOrDefaultAsync(cancellationToken);


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
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new Users
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                };

                _db.Users.Add(newUser);
                await _db.SaveChangesAsync(cancellationToken);

            }

            return new UsersRegistrationResponse
            {
                Message = "Success"
            };
        }

        public async Task<UsersLoginResponse> Login(UsersLoginRequest request, CancellationToken cancellationToken)
        {
            var checkUsernamel = await _db.Users.FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);

            if (checkUsernamel == null || !BCrypt.Net.BCrypt.Verify(request.Password, checkUsernamel.PasswordHash))
            {
                return new UsersLoginResponse
                {
                    Message = "Email / Password is incorrect"
                };
            }
            else
            {
                var token = createToken(checkUsernamel);
                return new UsersLoginResponse
                {
                    Message = "Success",
                    Token = token
                };
            }
        }

        private string createToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
