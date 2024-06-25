namespace CraApp.Repository.IRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);
}
