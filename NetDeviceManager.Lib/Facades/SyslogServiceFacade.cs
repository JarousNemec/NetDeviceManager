using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class SyslogServiceFacade(SyslogService syslogService, ILogger<SyslogService> logger) : ISyslogService
{
    public int GetSyslogAlertCount()
    {
        var result = syslogService.GetSyslogAlertCount();
        logger.LogInformation($"In system are {result} syslog Alerts in system");
        return result;
    }

    public int GetCurrentDeviceSyslogAlertsCount(Guid id)
    {
        var result = syslogService.GetCurrentDeviceSyslogAlertsCount(id);
        logger.LogInformation($"In system are {result} syslog Alerts for device id: {id}");
        return result;
    }

    public List<SyslogRecord> GetLastSyslogRecords(int count)
    {
        var result = syslogService.GetLastSyslogRecords(count);
        logger.LogInformation($"Got {result.Count} last syslog records from system");
        return result;
    }

    public Guid AddSyslogRecord(SyslogRecord record)
    {
        var result = syslogService.AddSyslogRecord(record);
        logger.LogInformation($"Added syslog record with id: {result}");
        return result;
    }

    public List<Guid> GetAllSyslogs()
    {
        var result = syslogService.GetAllSyslogs();
        logger.LogInformation($"Got {result.Count} syslog records from system");
        return result;
    }

    public List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count = -1)
    {
        var result = syslogService.GetSyslogRecordsWithFilter(model, count);
        logger.LogInformation($"Got {result.Count} syslog records with filter: DeviceName - {model.DeviceName}, IpAddresses - {model.IpAddresses}, Facility - {model.Facility}, Severity - {model.Severity}");
        return result;
    }

    public List<SyslogRecord> GetSyslogRecordsWithUnknownSource(int count = -1)
    {
        var result = syslogService.GetSyslogRecordsWithUnknownSource(count);
        logger.LogInformation($"Got {result.Count} syslog records with unknown source");
        return result;
    }
}