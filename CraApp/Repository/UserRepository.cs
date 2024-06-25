namespace CraApp.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}
