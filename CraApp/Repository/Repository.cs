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
        await dbSet.AddAsync(entity, cancellationToken);
        await SaveAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        dbSet.Remove(entity);
        await SaveAsync();
    }

    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbSet.ToListAsync(cancellationToken);
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

 
}
