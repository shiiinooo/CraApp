namespace CraApp.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    internal DbSet<T> dbSet;
    private readonly AppDbContext _db;

    public Repository(AppDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }
    public async Task CreateAsync(T entity, CancellationToken cancellationToken)
    {
        await dbSet.AddAsync(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        dbSet.Remove(entity);
        await SaveAsync(cancellationToken);
    }

    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbSet.ToListAsync();
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync();
    }
}
