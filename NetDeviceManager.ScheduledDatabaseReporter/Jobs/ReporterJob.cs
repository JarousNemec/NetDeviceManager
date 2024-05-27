using System.Text.Json;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using Quartz;

namespace ScheduledDatabaseReporter.Jobs;

public class ReporterJob : IJob
{
    private string _id = string.Empty;
    private string _path = string.Empty;
    private readonly IDatabaseService _databaseService;
    private const string SYSLOG_REPORT_FILENAME = "syslogs.json";
    private const string SNMP_REPORT_FILENAME = "snmps.json";

    public ReporterJob(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        InitializeJob(context);
        await Console.Out.WriteLineAsync("Reporting data bro.......");

        var devices = _databaseService.GetPhysicalDevices();
        foreach (var device in devices)
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            
            var currentdatepath = Path.Combine(_path, DateTime.Now.ToString("MM.dd.yyyy HH-mm-ss"));
            if (!Directory.Exists(currentdatepath))
                Directory.CreateDirectory(currentdatepath);
            
            var devicedirpath = Path.Combine(currentdatepath, device.IpAddress);
            if (!Directory.Exists(devicedirpath))
                Directory.CreateDirectory(devicedirpath);

            var snmppath = Path.Combine(devicedirpath, SNMP_REPORT_FILENAME);
            var syslogpath = Path.Combine(devicedirpath, SYSLOG_REPORT_FILENAME);

            var syslogs =
                _databaseService.GetSyslogRecordsWithFilter(
                    new SyslogRecordFilterModel() { IpAddress = device.IpAddress }, -1);
            await File.WriteAllTextAsync(syslogpath, JsonSerializer.Serialize(syslogs));

            var snmps = _databaseService.GetSnmpRecordsWithFilter(
                new SnmpRecordFilterModel() { IpAddress = device.IpAddress }, -1);

            await File.WriteAllTextAsync(snmppath, JsonSerializer.Serialize(snmps));
        }
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