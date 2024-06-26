﻿using CraApp.Model;
using Microsoft.EntityFrameworkCore;

namespace CraApp.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _db.Entry(user).State = EntityState.Modified;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> GetByIdWithActivitiesAsync(int userId, CancellationToken cancellationToken)
    {
        return await _db.Set<User>()
            .Include(u => u.MonthlyActivities)
                .ThenInclude(ma => ma.Activities)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

}
