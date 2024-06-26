namespace CraApp.Repository.IRepository;

public interface IMonthlyActivitiesRepository : IRepository<MonthlyActivities>
{

    Task<List<MonthlyActivities>> GetAllAsyncIncludeActivities(CancellationToken cancellationToken);
    Task<MonthlyActivities >GetByIdAsync(int monthlyActivitiesId);
}
