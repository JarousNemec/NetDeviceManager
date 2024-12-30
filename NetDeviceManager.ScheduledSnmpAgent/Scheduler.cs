using NetDeviceManager.Database;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Facades;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.Lib.Utils;
using NetDeviceManager.ScheduledSnmpAgent.Factories;
using NetDeviceManager.ScheduledSnmpAgent.Jobs;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Timer = System.Timers.Timer;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Scheduler
{
    private readonly IDatabaseService _databaseService;
    private readonly ILoginProfileService _loginProfileService;
    private readonly IPortService _portService;
    private readonly Timer _timer;
    private IScheduler _scheduler;
    private readonly IDeviceService _deviceService;

    public Scheduler(IDeviceService deviceService,IDatabaseService databaseService, Timer timer, ILoginProfileService loginProfileService, IPortService portService)
    {
        _databaseService = databaseService;
        _timer = timer;
        _loginProfileService = loginProfileService;
        _portService = portService;
        _deviceService = deviceService;
        SetupTimer(20000); //todo: change for production
        SetupScheduler();
    }

    private void SetupScheduler()
    {
        _scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        var connectionString = SystemConfigurationHelper.GetConnectionString();
        var serviceProvider = SetupServiceCollection(connectionString);
        _scheduler.JobFactory = new DIJobFactory(serviceProvider);
        _scheduler.Start();
    }

    private ServiceProvider SetupServiceCollection(string? connectionString)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ReadDeviceSensorsJob>();
        serviceCollection.AddScoped<ISnmpService, SnmpServiceFacade>();
        serviceCollection.AddScoped<SnmpService>();
        serviceCollection.AddScoped<IDatabaseService, DatabaseServiceFacade>();
        serviceCollection.AddScoped<DatabaseService>();
        serviceCollection.AddScoped<ISettingsService>();
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
        ScheduleJobs();
    }

    private async void ScheduleJobs()
    {
        var registeredJobs = _databaseService.GetSchedulerJobs();
        var group = SystemConfigurationHelper.GetValue("JobGroupName");
        if (group == null)
        {
            Console.Out.WriteLine("Cannot load configuration for readers group name");
            return;
        }

        await StartNewJobsIfThereAre(group, registeredJobs);
        await KillOldJobsIfThereAre(group, registeredJobs);
    }

    private async Task StartNewJobsIfThereAre(string group, List<SchedulerJob> registeredJobs)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        foreach (var registeredJob in registeredJobs)
        {
            if (jobKeys.All(x => x.Name != registeredJob.Id.ToString()))
            {
                if (registeredJob.Type == SchedulerJobType.SNMPGET)
                {
                    await ScheduleSnmpGetJob(registeredJob, group);
                    
                }
            }
        }
    }
    private async Task KillOldJobsIfThereAre(string group, List<SchedulerJob> registeredJobs)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        foreach (var jobKey in jobKeys)
        {
            if (registeredJobs.All(x => x.Id.ToString() != jobKey.Name))
            {
                await _scheduler.DeleteJob(jobKey);
            }
        }
    }

    private async Task ScheduleSnmpGetJob(SchedulerJob registeredJob, string readersGroup)
    {
        var port = SnmpUtils.GetSnmpPort(registeredJob.PhysicalDeviceId, _portService);
        if (port == null) return;
        var loginProfile = _loginProfileService.GetPhysicalDeviceLoginProfiles(registeredJob.PhysicalDevice.Id);
        string id = registeredJob.Id.ToString();

        var sensorsInPhysicalDevice = _deviceService.GetSensorsOfPhysicalDevice(registeredJob.PhysicalDeviceId);

        if (sensorsInPhysicalDevice.Any())
        {
            var job = SchedulerUtil.CreateReadDeviceSensorJob(id, registeredJob.PhysicalDevice,
                sensorsInPhysicalDevice, port, loginProfile,
                readersGroup);
            var trigger = SchedulerUtil.CreateJobTrigger(id, registeredJob.Cron,
                readersGroup);

            await _scheduler.ScheduleJob(job, trigger);
        }
    }
}