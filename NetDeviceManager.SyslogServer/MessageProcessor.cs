using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.Lib.Utils;
using NetDeviceManager.SyslogServer.Models;

namespace NetDeviceManager.SyslogServer;

//config keys: DesiredSeverities
public class MessageProcessor
{
    private readonly ServerCache _cache;
    private readonly IDeviceService _deviceService;
    private readonly ISyslogService _syslogService;
    private readonly IDatabaseService _databaseService;
    private readonly ISettingsService _settingsService;
    // private readonly SettingsService _settingsService;

    public delegate void CrashDelegate(string m);

    public event CrashDelegate? OnCrash;

    public MessageProcessor(ServerCache cache, string dbConnString)
    {
        _cache = cache;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(dbConnString)
            .Options;

        var context = new ApplicationDbContext(options);
        _settingsService = new SettingsService(context);
        _databaseService = new DatabaseService(context);
        _deviceService = new DeviceService(context, _databaseService, _settingsService);
        _syslogService = new SyslogService(context);
        // _settingsService = new SettingsService(_database);
    }

    public async void Run()
    {
        Console.WriteLine("Running processor...");
        try
        {
            while (true)
            {
                Thread.Sleep(1000);
                lock (_cache.ProcessorLock)
                {
                    if (_cache.MessagesQueue == null || _cache.MessagesQueue.Count < 1)
                    {
                        continue;
                    }

                    foreach (var message in _cache.MessagesQueue)
                    {
                        Console.WriteLine($"Processing message.... ({message.Ip}) : {message.Message}");
                        var record = ParseRecord(message);

                        _syslogService.AddSyslogRecord(record);
                    }

                    _cache.MessagesQueue = new List<CacheMessageModel>();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Processor has crashed with message " + e);
            Debug.WriteLine("Processor has crashed with message " + e);
            Thread.Sleep(5000);
            OnCrash?.Invoke(e.Message);
        }
    }

    private SyslogRecord ParseRecord(CacheMessageModel message)
    {
        var messageValue = message.Message;
        var device = _deviceService.GetPhysicalDeviceByIp(message.Ip);
        var format = SyslogUtil.IdentifySyslogFormat(messageValue);
        var creationDate = SyslogUtil.GetSyslogTimestamp(messageValue, format);
        var priority = SyslogUtil.GetSyslogPriority(messageValue);

        var facility = SyslogFacility.LocalUse0;
        var severity = SyslogSeverity.Informational;

        if (priority != int.MinValue)
        {
            facility = (SyslogFacility)(priority / 8);
            severity = (SyslogSeverity)(priority % 8);
        }
        else if (format == SyslogFormat.Cisco)
        {
            severity = SyslogUtil.ParseCiscoSyslogSeverity(messageValue);
        }

        var record = new SyslogRecord()
        {
            CompletMessage = messageValue,
            Facility = facility,
            Severity = severity,
            Message = string.Empty,
            PhysicalDeviceId = device?.Id,
            ProcessedDate = DateTime.Now,
            CreationDate = creationDate,
            Ip = message.Ip
        };

        return record;
    }
}