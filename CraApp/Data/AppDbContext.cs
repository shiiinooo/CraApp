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
    public DbSet<MonthlyActivities> MonthlyActivities{ get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>().Property(e => e.StartTime).HasColumnType("time(0)");
        modelBuilder.Entity<Activity>().Property(e => e.EndTime).HasColumnType("time(0)");
        

        modelBuilder.Entity<User>().HasData(

            new User()
            {
                Id = 1,
                UserName = "shiinoo",
                Name = "Ahmed",
                Password = "Password123#",
                Role = Role.admin
            },
            new User()
            {
                Id = 2,
                UserName = "PipInstallGeek",
                Name = "Marouane",
                Password = "Password123#",
                Role = Role.admin
            });
        modelBuilder.Entity<MonthlyActivities>().HasData(

            new MonthlyActivities()
            {
                Id = 1,
                Year = 2024,
                Month = 01,
                UserId = 1
            },
             new MonthlyActivities()
             {
                 Id= 2,
                 Year = 2024,
                 Month = 02,
                 UserId = 1
             }

            );
        modelBuilder.Entity<Activity>().HasData(

            new Activity
            {
                Id = 1,
                StartTime = new TimeSpan(10, 00, 00),
                EndTime = new TimeSpan(16, 00, 00),
                Day = 1,
                Project = Project.Formation,
                MonthlyActivitiesId = 1
            },
             new Activity
             {
                 Id = 21,
                 StartTime = new TimeSpan(10, 00, 00),
                 EndTime = new TimeSpan(16, 00, 00),
                 Day = 2,
                 Project = Project.MyTaraji,
                 MonthlyActivitiesId = 1
             },
              new Activity
              {
                  Id = 3,
                  StartTime = new TimeSpan(10, 00, 00),
                  EndTime = new TimeSpan(16, 00, 00),
                  Day = 3,
                  Project = Project.Formation,
                  MonthlyActivitiesId = 2
              }

            );
    }

}

