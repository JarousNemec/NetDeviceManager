using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.SyslogServer.Models;

namespace NetDeviceManager.SyslogServer;

//config keys: DesiredSeverities
public class MessageProcessor
{
    private readonly ServerCache _cache;
    private readonly IDatabaseService _database;
    private readonly SettingsService _settingsService;

    public MessageProcessor(ServerCache cache, string dbConnString)
    {
        _cache = cache;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(dbConnString)
            .Options;
            
        var context = new ApplicationDbContext(options);
        _database = new DatabaseService(context);
        _settingsService = new SettingsService(_database);
    }

    public async void Run()
    {
        Console.WriteLine("Running processor...");
        try
        {
            var severities = _settingsService.GetSettings().DesiredSeverities;

            while (true)
            {
                Thread.Sleep(1000);
                lock (_cache.ProcessorLock)
                {
                    if (_cache.MessagesQueue == null || _cache.MessagesQueue.Count < 1)
                    {
                        continue;
                    }

                    foreach (var Message in _cache.MessagesQueue)
                    {
                        Console.WriteLine($"Processing message.... ({Message.Ip}) : {Message.Message}");
                        var record = ParseRecord(Message);
                        
                        if (severities.Contains(record.Severity) || record.Severity == -1)
                        {
                            _database.AddSyslogRecord(record);
                        }
                    }
                    _cache.MessagesQueue = new List<CacheMessageModel>();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Thread.Sleep(5000);
            // Console.WriteLine("New atempt to run processor...");
            // Run();
        }
    }
    private SyslogRecord ParseRecord(CacheMessageModel message)
    {
        var device = _database.GetPhysicalDeviceByIp(message.Ip);

        SyslogRecord record;
        // switch (_database.GetConfigValue("SyslogFormat")?.ToLower())
        // {
        //     case "cisco":
        //         record = CiscoFormatParseRecord(message, device);
        //         break;
        //     default:
        //         record = CiscoFormatParseRecord(message, device);
        //         break;
        // }
        record = CiscoFormatParseRecord(message, device);
        return record;

    }

    private SyslogRecord CiscoFormatParseRecord(CacheMessageModel message, PhysicalDevice? device)
    {
        var priority = CiscoFormatParsePriority(message.Message);
        int facility = priority / 8;
        int severity = priority % 8;
        if (priority == -1)
        {
            severity = CiscoFormatParseSeverityFromText(message.Message);
        }

        if (severity == -1)
        {
            facility = -1;
        }
        var record = new SyslogRecord()
        {
            CompletMessage = message.Message,
            Facility = facility,
            Severity = severity,
            Message = message.Message.Split('%').Last(),
            PhysicalDeviceId = device?.DeviceId,
            ProcessedDate = DateTime.Now,
            Ip = message.Ip
        };
        return record;
    }

    private int CiscoFormatParseSeverityFromText(string message)
    {
        var step1 = message.Split('%').Last();
        var step2 = step1.Split(':').First();
        var step3 = step2.Split('-')[1];
        return int.TryParse(step3, out int severity) ? severity : -1;
    }

    private int CiscoFormatParsePriority(string message)
    {
        var startIndex = 0;
        string number = "";
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == '<')
            {
                startIndex = i + 1;
                break;
            }
        }

        for (int i = startIndex; i < message.Length; i++)
        {
            if (message[i] != '>')
            {
                number += message[i];
            }
            else
            {
                break;
            }
        }

        return int.TryParse(number, out int priority) ? priority : -1;
    }
}