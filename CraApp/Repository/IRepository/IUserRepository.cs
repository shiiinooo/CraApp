namespace CraApp.Repository.IRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken);
}
