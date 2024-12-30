using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;
using Quartz;

namespace ScheduledDatabaseReporter.Jobs;

public class ReporterJob : IJob
{
    private string _id = string.Empty;
    private string _path = string.Empty;
    private readonly IDeviceService _deviceService;
    private readonly ISyslogService _syslogService;
    private const string SYSLOG_REPORT_FILENAME = "syslogs.log";
    private const int SYSLOG_OLDER_THAN_DAYS = 30;

    public ReporterJob(ISyslogService syslogService, IDeviceService deviceService)
    {
        _deviceService = deviceService;
        _syslogService = syslogService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        InitializeJob(context);

        await Console.Out.WriteLineAsync("Reporting data bro.......");

        var date = DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");
        var currentDatePath = FileUtil.PrepareDiskForReportingLogs(_path, date);

        await GenerateReports(currentDatePath);

        var zipPath = FileUtil.GetArchivePath(_path, date);
        FileUtil.ArchiveDirectory(zipPath, currentDatePath);
        Directory.Delete(currentDatePath, true);
    }

    private async Task GenerateReports(string currentdatepath)
    {
        var devices = _deviceService.GetAllPhysicalDevices();
        foreach (var device in devices)
        {
            await ReportSyslogsOfDevice(device, currentdatepath);
        }

        await ReportSyslogsWithUnknownSources(currentdatepath);
    }


    private async Task ReportSyslogsOfDevice(PhysicalDevice device, string currentdatepath)
    {
        var ipAddresses = _deviceService.GetPhysicalDeviceIpAddresses(device.Id);
        var syslogs =
            _syslogService.GetSyslogRecordsWithFilter(
                new SyslogRecordFilterModel() { IpAddresses = String.Join(";", ipAddresses) });

        if (syslogs.Count == 0)
            return;
        var oldSyslogs = SyslogUtil.FilterOlderSyslogs(syslogs, DateTime.Now, SYSLOG_OLDER_THAN_DAYS);
        if (oldSyslogs.Count == 0)
            return;

        var filepath = FileUtil.PrepareReportEnvironment(device.Name, currentdatepath, SYSLOG_REPORT_FILENAME);
        await FileUtil.WriteSyslogsToFile(filepath, oldSyslogs);

        _deviceService.DeleteSyslogsOfDevice(device.Id);
    }

    private async Task ReportSyslogsWithUnknownSources(string currentdatepath)
    {
        var syslogs =
            _syslogService.GetSyslogRecordsWithUnknownSource();

        if (syslogs.Count == 0)
            return;
        var oldSyslogs = SyslogUtil.FilterOlderSyslogs(syslogs, DateTime.Now, SYSLOG_OLDER_THAN_DAYS);
        if (oldSyslogs.Count == 0)
            return;

        var filepath = FileUtil.PrepareReportEnvironment("UnknownSource", currentdatepath, SYSLOG_REPORT_FILENAME);
        await FileUtil.WriteSyslogsToFile(filepath, oldSyslogs, true);

        _deviceService.DeleteSyslogsOfDevice(null);
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