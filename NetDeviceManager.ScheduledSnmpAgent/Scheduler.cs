using NetDeviceManager.Database;
using System.Timers;
using FluentScheduler;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.ScheduledSnmpAgent.Jobs;
using NCrontab;
using Timer = System.Timers.Timer;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Scheduler
{
    private readonly ApplicationDbContext _database;
    private readonly Timer _timer;

    public Scheduler(ApplicationDbContext database, Timer timer)
    {
        _database = database;
        _timer = timer;
        _timer.Interval = 10000;
        _timer.Elapsed += TimerOnElapsed;
        _timer.Start();
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Console.WriteLine("Watching for new schedulers...");
        Schedule();
    }

    public void Schedule()
    {
        var intervals = _database.PhysicalDevicesReadJobs.ToList();
        var runningSchedulers = JobManager.RunningSchedules.ToList();
        foreach (var jobInterval in intervals)
        {
            if (runningSchedulers.All(x => x.Name != jobInterval.Id.ToString()))
            {
                // JobManager.AddJob(
                //     new ReadDeviceSensorsJob("1",new List<SnmpSensor>(), new PhysicalDevice()),
                //     schedule =>
                //     {
                //         jobInterval.SchedulerCron;
                //         schedule.WithName(jobInterval.Id.ToString());
                //     });
                // Schedule job = new Schedule(() => new ReadDeviceSensorsJob("1",new List<SnmpSensor>(), new PhysicalDevice()));
                // var schedule = new JobStartInfo().StartTime = 
                // JobManager.AddJob<ReadDeviceSensorsJob>(,schedule);
            }
        }
    }
}