using NetDeviceManager.Database;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.Lib.Utils;
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
    private readonly SettingsService _settingsService;
    private string _actualCron = string.Empty;

    public Scheduler(IDatabaseService databaseService, Timer timer, SettingsService settingsService)
    {
        _databaseService = databaseService;
        _timer = timer;
        _settingsService = settingsService;
        SetupTimer(20000); //todo: change for production
        SetupScheduler();
    }

    private void SetupScheduler()
    {
        _scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        var connectionString = ConfigurationHelper.GetConnectionString();
        var serviceProvider = SetupServiceCollection(connectionString);
        _scheduler.JobFactory = new DIJobFactory(serviceProvider);
        _scheduler.Start();
    }

    private ServiceProvider SetupServiceCollection(string? connectionString)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ReadDeviceSensorsJob>();
        serviceCollection.AddScoped<ISnmpService, SnmpService>();
        serviceCollection.AddScoped<IDatabaseService, DatabaseService>();
        serviceCollection.AddScoped<SettingsService>();
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
        var reportCron = _settingsService.GetSettings().ReportSensorInterval;
        var group = ConfigurationHelper.GetValue("JobGroupName");
        var devices = _databaseService.GetPhysicalDevices();
        if (group == null)
        {
            Console.Out.WriteLine("Cannot load configuration for readers group name");
            return;
        }

        if (reportCron != _actualCron)
        {
            await KillOldReportJobs(group);
            _actualCron = reportCron;
            Console.WriteLine($"New cron is: {_actualCron}");
        }

        await StartNewJobsIfThereAre(group, reportCron, devices);
        await KillOldJobsIfThereAre(group, devices);
    }
    private async Task KillOldReportJobs(string group)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        foreach (var jobKey in jobKeys)
        {
            await _scheduler.DeleteJob(jobKey);
            Console.WriteLine($"Old job interrupted...");
        }
    }

    private async Task StartNewJobsIfThereAre(string group, string cron, List<PhysicalDevice> devices)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        foreach (var device in devices)
        {
            if (jobKeys.All(x => x.Name != device.Id.ToString()))
            {
                await ScheduleSnmpGetJob(cron, device, group);
            }
        }
    }

    private async Task KillOldJobsIfThereAre(string group, List<PhysicalDevice> devices)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        foreach (var jobKey in jobKeys)
        {
            if (devices.All(x => x.Id.ToString() != jobKey.Name))
            {
                await _scheduler.DeleteJob(jobKey);
            }
        }
    }

    private async Task ScheduleSnmpGetJob(string cron, PhysicalDevice device, string readersGroup)
    {
        var port = SnmpUtils.GetSnmpPort(device.Id, _databaseService);
        if (port == null) return;
        var loginProfile = _databaseService.GetLoginProfile(device.LoginProfileId);
        if (loginProfile == null)
            return;

        var job = SchedulerUtil.CreateReadDeviceSensorJob(device.Id.ToString(), device, port, loginProfile,
            readersGroup);
        var trigger = SchedulerUtil.CreateJobTrigger(device.Id.ToString(), cron,
            readersGroup);

        await _scheduler.ScheduleJob(job, trigger);
    }
}