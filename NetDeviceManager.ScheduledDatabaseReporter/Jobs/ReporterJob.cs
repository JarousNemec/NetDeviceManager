using NetDeviceManager.Lib.Interfaces;
using Quartz;

namespace ScheduledDatabaseReporter.Jobs;

public class ReporterJob : IJob
{
    private string _id = string.Empty;
    private string _path = string.Empty;
    private readonly IDatabaseService _databaseService;

    public ReporterJob(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        InitializeJob(context);

        /////////////////////////////
        await Console.Out.WriteLineAsync("Reporting data bro.......");
        // if (!Directory.Exists(_path))
        //     Directory.CreateDirectory(_path);
        // File.Create(Path.Combine(_path, $"{DateTime.Now}.report"));
    }

    private void InitializeJob(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        _id = (string)dataMap["id"];
        _path = (string)dataMap["path"];

        Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Job started...");
        Console.Out.WriteLine($"Path for reporting is: {_path}");
    }

}