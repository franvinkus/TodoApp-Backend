using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using TodoApp_Backend.Data;
using TodoApp_Backend.Models;
using TodoApp_Backend.Repositories.Interface;

namespace TodoApp_Backend.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly TodoAppDbContext _db;
        public UserRepository(TodoAppDbContext db)
        {
            _db = db;
        }
        public async Task<Users?> IsUserExist(string username, string email, CancellationToken cancellationToken)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username || x.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<Users?> CheckUserByUsername(string username, CancellationToken cancellationToken)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        }

        public void AddUser(Users user)
        {
            _db.Users.Add(user);
        }

        public void RemoveUser(Users user)
        {
            _db.Users.Remove(user);
        }

        public async Task SaveUser(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

    }
}
