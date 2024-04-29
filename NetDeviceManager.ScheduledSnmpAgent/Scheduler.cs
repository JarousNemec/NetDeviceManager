using NetDeviceManager.Database;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.ScheduledSnmpAgent.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using Timer = System.Timers.Timer;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Scheduler
{
    private readonly ApplicationDbContext _database;
    private readonly Timer _timer;
    private readonly IScheduler _scheduler;
    private const string SNMP_READERS_GROUPNAME = "readers";

    public Scheduler(ApplicationDbContext database, Timer timer)
    {
        _database = database;
        _timer = timer;
        _timer.Interval = 10000;
        _timer.Elapsed += TimerOnElapsed;
        _timer.Start();

        _scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        _scheduler.Start();
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Console.WriteLine("Watching for new schedulers...");
        Schedule();
    }

    public async void Schedule()
    {
        var registeredJobs = _database.PhysicalDevicesReadJobs.Include(x => x.PhysicalDevice).ToList();
        var jobKeys = _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(SNMP_READERS_GROUPNAME)).Result;
        foreach (var registeredJob in registeredJobs)
        {
            if (jobKeys.All(x => x.Name != registeredJob.Id.ToString()))
            {
                var sensorsInPhysicalDevice = _database.SensorsInPhysicalDevices
                    .Where(x => x.PhysicalDeviceId == registeredJob.PhysicalDeviceId).Include(x => x.SnmpSensor).Include(x => x.SnmpSensor.Community)
                    .ToList();
                if (!sensorsInPhysicalDevice.Any())
                {
                    return;
                }
                
                
                string id = registeredJob.Id.ToString();
                PhysicalDevice physicalDevice = registeredJob.PhysicalDevice;

                IJobDetail job = JobBuilder.Create<ReadDeviceSensorsJob>()
                    .WithIdentity($"{id}", SNMP_READERS_GROUPNAME).Build();
                    
                job.JobDataMap.Put("id", id);
                job.JobDataMap.Put("physicalDevice", physicalDevice);
                job.JobDataMap.Put("sensors", sensorsInPhysicalDevice);
                job.JobDataMap.Put("database", _database);
                
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"T_{id}", SNMP_READERS_GROUPNAME)
                    .StartNow()
                    .WithCronSchedule(registeredJob.SchedulerCron)
                    .Build();
                await _scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}