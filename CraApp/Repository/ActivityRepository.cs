namespace CraApp.Repository;

public class ActivityRepository : Repository<Activity>, IActivityRepository
{
    private readonly AppDbContext _db;
    public ActivityRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}
