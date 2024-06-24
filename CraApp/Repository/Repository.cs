using CraApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CraApp.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    internal DbSet<T> dbSet;
    private readonly AppDbContext _db;

    public Repository(AppDbContext db)
    {
        dbSet = _db.Set<T>();
        _db = db;
    }
    public async Task CreateAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        await SaveAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        dbSet.Remove(entity);
        await SaveAsync();
    }

    public Task<List<T>> GetAllAsync()
    {
        return dbSet.ToListAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}
