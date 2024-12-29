using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class SyslogService : ISyslogService
{
    private readonly ApplicationDbContext _database;
    public SyslogService(ApplicationDbContext database)
    {
        _database = database;
    }
    private readonly List<Guid> _devicesSyslogAlerts = new();
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);
    public int GetSyslogAlertCount()
    {
        CheckTimelinessOfData();
        return _devicesSyslogAlerts.Count;
    }

    public int GetCurrentDeviceSyslogAlertsCount(Guid id)
    {
        CheckTimelinessOfData();
        return _devicesSyslogAlerts.Count(x => x == id);
    }

    private void CheckTimelinessOfData()
    {
        if ((DateTime.Now.Ticks - _lastUpdate.Ticks) > (TimeSpan.TicksPerMinute * 5))
        {
            _devicesSyslogAlerts.Clear();
            _devicesSyslogAlerts.AddRange(GetAllSyslogs());
            _lastUpdate = DateTime.Now;
        }
    }
    
    public List<SyslogRecord> GetLastSyslogRecords(int count)
    {
        var data = _database.SyslogRecords.AsNoTracking().Include(x => x.PhysicalDevice)
            .OrderByDescending(x => x.ProcessedDate).Take(count).ToList();
        return data;
    }

    public Guid AddSyslogRecord(SyslogRecord record)
    {
        var id = DatabaseUtil.GenerateId();
        record.Id = id;
        _database.SyslogRecords.Add(record);
        _database.SaveChanges();
        return id;
    }

    public List<Guid> GetAllSyslogs()
    {
        return _database.SyslogRecords.AsNoTracking().Select(x => x.Id).ToList();
    }

    public List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count = -1)
    {
        IQueryable<SyslogRecord> query = _database.SyslogRecords.AsNoTracking().Include(x => x.PhysicalDevice);
        if (!string.IsNullOrEmpty(model.DeviceName))
        {
            query = query.Where(x => x.PhysicalDevice.Name == model.DeviceName);
        }

        if (!string.IsNullOrEmpty(model.IpAddresses))
        {
            List<string> ipAddresses = new List<string>();

            var ips = model.IpAddresses.Split(';');
            foreach (var ip in ips)
            {
                if (IPAddress.TryParse(ip, out IPAddress? output))
                {
                    ipAddresses.Add(output.ToString());
                }
            }

            query = query.Where(x => ipAddresses.Any(y => y == x.Ip));
        }

        if (model.Facility >= 0 && model.Facility != SyslogFacility.Undefined)
        {
            query = query.Where(x => x.Facility == model.Facility);
        }

        if (model.Severity >= 0 && model.Severity != SyslogSeverity.Undefined)
        {
            query = query.Where(x => x.Severity == model.Severity);
        }

        if (count == -1)
            return query.OrderByDescending(x => x.ProcessedDate).ToList();
        return query.OrderByDescending(x => x.ProcessedDate).Take(count).ToList();
    }

    public List<SyslogRecord> GetSyslogRecordsWithUnknownSource(int count = -1)
    {
        var records = _database.SyslogRecords.AsNoTracking().Where(x => x.PhysicalDeviceId == null);
        if (count == -1)
            return records.ToList();
        return records.Take(count).ToList();
    }
}