namespace CraApp.Repository.IRepository;

public interface IActivityRepository : IRepository<Activity>
{
    Task<Activity> GetById(int Id);
}
