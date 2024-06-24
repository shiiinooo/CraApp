namespace CraApp.Repository.IRepository;

public interface IUserRepository
{
    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);
}
