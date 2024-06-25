namespace CraApp.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public DbSet<Activity> Activities { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(

            new User()
            {
                Id = 1,
                UserName = "shiinoo",
                Name = "Ahmed",
                Password = "Password123#",
                Role = "admin"
            },
            new User()
            {
                Id = 2,
                UserName = "PipInstallGeek",
                Name = "Marouane",
                Password = "Password123#",
                Role = "admin"
            });
        modelBuilder.Entity<Activity>().HasData(
            new Activity()
            {
                Id = 1,
                Project = Project.MyTaraji,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
            });

    }

}

