using System.IO.Compression;
using System.Text;
using System.Text.Json;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Utils;
using Quartz;

namespace ScheduledDatabaseReporter.Jobs;

public class ReporterJob : IJob
{
    private string _id = string.Empty;
    private string _path = string.Empty;
    private readonly IDatabaseService _databaseService;
    private const string SYSLOG_REPORT_FILENAME = "syslogs.log";

    public ReporterJob(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        InitializeJob(context);
        await Console.Out.WriteLineAsync("Reporting data bro.......");

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        var date = DateTime.Now;

        var currentdatepath = Path.Combine(_path, date.ToString("MM.dd.yyyy HH-mm-ss"));
        if (!Directory.Exists(currentdatepath))
            Directory.CreateDirectory(currentdatepath);

        var devices = _databaseService.GetPhysicalDevices();
        foreach (var device in devices)
        {
            var devicedirpath = PrepareReportDirectoryPath(device, currentdatepath);

            var syslogpath = Path.Combine(devicedirpath, SYSLOG_REPORT_FILENAME);

            ReportAllSyslogs(device, syslogpath);
        }

        _databaseService.DeleteAllSyslogs();

        var zipPath = Path.Combine(_path, $"{date.ToString("MM.dd.yyyy HH-mm-ss")}.zip");
        FileUtil.ZipDirectory(currentdatepath, zipPath);
        Directory.Delete(currentdatepath);
    }


    private string PrepareReportDirectoryPath(PhysicalDevice device, string currentdatepath)
    {
        var devicedirpath = Path.Combine(currentdatepath, device.IpAddress);
        if (!Directory.Exists(devicedirpath))
            Directory.CreateDirectory(devicedirpath);
        return devicedirpath;
    }

    private async void ReportAllSyslogs(PhysicalDevice device, string syslogpath)
    {
        var syslogs =
            _databaseService.GetSyslogRecordsWithFilter(
                new SyslogRecordFilterModel() { IpAddress = device.IpAddress }, -1);
        StringBuilder builder = new StringBuilder();
        foreach (var log in syslogs)
        {
            builder.AppendLine($"{log.ProcessedDate.ToString("MM.dd.yyyy HH-mm-ss")} - {log.CompletMessage}");
        }

        var data = builder.ToString();
        if (string.IsNullOrEmpty(data))
            data = "No new logs...";
        await File.WriteAllTextAsync(syslogpath, data);
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