namespace TodoApp_Backend.Services.Interface
{
    public interface ICryptographyService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
