using NetDeviceManager.Database;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.Database.Services;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Snmp.Interfaces;
using NetDeviceManager.Lib.Snmp.Services;
using NetDeviceManager.ScheduledSnmpAgent.Factories;
using NetDeviceManager.ScheduledSnmpAgent.Helpers;
using NetDeviceManager.ScheduledSnmpAgent.Jobs;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Timer = System.Timers.Timer;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Scheduler
{
    private readonly IDatabaseService _databaseService;
    private readonly Timer _timer;
    private IScheduler _scheduler;

    public Scheduler(IDatabaseService databaseService, Timer timer)
    {
        _databaseService = databaseService;
        _timer = timer;
        SetupTimer(20000); //todo: change for production
        SetupScheduler();
    }

    private void SetupScheduler()
    {
        _scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        var connectionString = ConfigurationHelper.GetConfigurationString();
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
        ScheduleJobs();
    }

    private async void ScheduleJobs()
    {
        var registeredJobs = _databaseService.GetSnmpReadJobs();
        var group = ConfigurationHelper.GetValue("JobGroupName");
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
                await _scheduler.Interrupt(jobKey);
            }
        }
    }

    private async Task ScheduleSnmpGetJob(SchedulerJob registeredJob, string readersGroup)
    {
        var port = _databaseService.GetPortInPhysicalDevices(registeredJob.PhysicalDeviceId)
            .FirstOrDefault(x => x.Port.Protocol == CommunicationProtocol.SNMP)?.Port;
        var loginProfile = _databaseService.GetPhysicalDeviceLoginProfile(registeredJob.PhysicalDevice.LoginProfileId);
        if (port == null)
        {
            return;
        }

        string id = registeredJob.Id.ToString();

        var sensorsInPhysicalDevice = _databaseService.GetSensorsOfPhysicalDevice(registeredJob.PhysicalDeviceId);

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