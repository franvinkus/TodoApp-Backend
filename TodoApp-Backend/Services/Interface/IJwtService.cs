using TodoApp_Backend.Models;

namespace TodoApp_Backend.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(Users user);
    }
}
