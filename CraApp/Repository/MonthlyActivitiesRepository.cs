

namespace CraApp.Repository;

public class MonthlyActivitiesRepository : Repository<MonthlyActivities>, IMonthlyActivitiesRepository
{
    private readonly AppDbContext _db;

    public MonthlyActivitiesRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public Task<List<MonthlyActivities>> GetAllAsyncIncludeActivities(CancellationToken cancellationToken)
    {
       return _db.MonthlyActivities.Include(m=> m.Activities).ToListAsync(cancellationToken);
    }

    public async Task<MonthlyActivities> GetByIdAsync(int monthlyActivitiesId)
    {
        return await _db.MonthlyActivities.FirstOrDefaultAsync(u => u.Id == monthlyActivitiesId);
    }
}
