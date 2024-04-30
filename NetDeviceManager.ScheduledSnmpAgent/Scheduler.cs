using NetDeviceManager.Database;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Snmp.Interfaces;
using NetDeviceManager.Lib.Snmp.Services;
using NetDeviceManager.ScheduledSnmpAgent.Factories;
using NetDeviceManager.ScheduledSnmpAgent.Helpers;
using NetDeviceManager.ScheduledSnmpAgent.Interfaces;
using NetDeviceManager.ScheduledSnmpAgent.Jobs;
using NetDeviceManager.ScheduledSnmpAgent.Services;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Timer = System.Timers.Timer;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Scheduler
{
    private readonly IDatabaseService _database;
    private readonly Timer _timer;
    private IScheduler _scheduler;

    public Scheduler(IDatabaseService database, Timer timer)
    {
        _database = database;
        _timer = timer;
        SetupTimer(20000); //todo: change for production
        SetupScheduler();
    }

    private void SetupScheduler()
    {
        _scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        var connectionString = ConfigurationHelper.GetConfigurationString("DefaultConnection");
        var serviceProvider = SetupServiceCollection(connectionString);
        _scheduler.JobFactory = new DIJobFactory(serviceProvider);
        _scheduler.Start();
    }

    private ServiceProvider SetupServiceCollection(string? connectionString)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ReadDeviceSensorsJob>();
        serviceCollection.AddScoped<ISnmpService, SnmpService>();
        serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider;
    }

    private void SetupTimer(int interval = 60000)
    {
        _timer.Interval = interval;
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
        ScheduleReadDeviceSensorJobs();
    }

    private async void ScheduleReadDeviceSensorJobs()
    {
        var registeredJobs = _database.GetSnmpReadJobs();
        var readersGroup = ConfigurationHelper.GetValue("SnmpReadersGroupName");
        if (readersGroup == null)
        {
            Console.Out.WriteLine("Cannot load configuration for readers group name");
            return;
        }

        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(readersGroup)).Result;
        foreach (var registeredJob in registeredJobs)
        {
            if (jobKeys.All(x => x.Name != registeredJob.Id.ToString()))
            {
                string id = registeredJob.Id.ToString();

                var sensorsInPhysicalDevice = _database.GetSensorsOfPhysicalDevice(registeredJob.PhysicalDeviceId);

                if (sensorsInPhysicalDevice.Any())
                {
                    var job = SchedulerUtil.CreateReadDeviceSensorJob(id, registeredJob.PhysicalDevice,
                        sensorsInPhysicalDevice,
                        readersGroup);
                    var trigger = SchedulerUtil.CreateJobTrigger(id, registeredJob.SchedulerCron,
                        readersGroup);

                    await _scheduler.ScheduleJob(job, trigger);
                }
            }
        }
    }
}