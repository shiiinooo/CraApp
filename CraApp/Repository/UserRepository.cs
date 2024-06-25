using Microsoft.EntityFrameworkCore;

namespace CraApp.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}
