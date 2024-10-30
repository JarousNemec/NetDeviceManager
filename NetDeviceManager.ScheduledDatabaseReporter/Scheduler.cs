using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using ScheduledDatabaseReporter.Factories;
using ScheduledDatabaseReporter.Helpers;
using ScheduledDatabaseReporter.Jobs;
using ScheduledDatabaseReporter.Utils;
using Timer = System.Timers.Timer;

namespace ScheduledDatabaseReporter;

public class Scheduler
{
    private readonly IDatabaseService _databaseService;
    private readonly Timer _timer;
    private IScheduler _scheduler;
    private const string _jobId = "myReportJob";
    private string _actualCron = string.Empty;
    private readonly SettingsService _settingsService;
    private readonly IHostEnvironment _environment;
    public Scheduler(IDatabaseService databaseService, Timer timer, SettingsService settingsService, IHostEnvironment environment)
    {
        _environment = environment;
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
        serviceCollection.AddScoped<IDatabaseService, DatabaseService>();
        serviceCollection.AddScoped<SettingsService>();
        serviceCollection.AddScoped<ReporterJob>();
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
        Console.WriteLine("Checking report scheduler job");
        Schedule();
    }

    public void Schedule()
    {
        ScheduleJob();
    }

    private async void ScheduleJob()
    {
        var group = ConfigurationHelper.GetValue("JobGroupName");
        if (group == null)
        {
            Console.Out.WriteLine("Cannot load configuration for readers group name");
            return;
        }

        var reportCron = _settingsService.GetSettings().ReportLogInterval;
        
        if (reportCron != _actualCron)
        {
            await KillOldReportJob(group);
            await StartNewReportJob(group, reportCron);
            _actualCron = reportCron;
            Console.WriteLine($"New cron is: {_actualCron}");
            
        }
    }

    private async Task StartNewReportJob(string group, string cron)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        if (jobKeys.All(x => x.Name != _jobId))
        {
            await ScheduleReportJob(cron, group);
            Console.WriteLine($"New job started with cron:{cron}");
        }
    }

    private async Task KillOldReportJob(string group)
    {
        var jobKeys = _scheduler
            .GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)).Result.ToList();
        foreach (var jobKey in jobKeys)
        {
            if (jobKeys.Any(x => x.Name == _jobId))
            {
                await _scheduler.DeleteJob(jobKey);
                Console.WriteLine($"Old job interrupted...");
            }
        }
    }

    private async Task ScheduleReportJob(string cron, string group)
    {
        var path = ConfigurationHelper.GetPath();
        
        if(string.IsNullOrEmpty(path))
            return;
        Console.WriteLine($"Path for new job is: {path}");
        Console.WriteLine($"Example path: {Path.Combine(path, "file.exe")}");
        var job = SchedulerUtil.CreateReportJob(_jobId, path,
            group);
        var trigger = SchedulerUtil.CreateJobTrigger(_jobId, cron,
            group);

        await _scheduler.ScheduleJob(job, trigger);
    }
}