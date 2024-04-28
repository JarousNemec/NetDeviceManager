using NetDeviceManager.Database;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Scheduler
{
    private readonly ApplicationDbContext _database;
    public Scheduler(ApplicationDbContext database)
    {
        _database = database;
    }

    public void Schedule()
    {
        Console.WriteLine($"count of communities: {_database.Communities.ToList().Count()}");
    }
}