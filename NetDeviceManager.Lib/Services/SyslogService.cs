using System.Text.Json;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;

namespace NetDeviceManager.Lib.Services;

public class SyslogService : ISyslogService
{
    private readonly IDatabaseService _database;
    private readonly int[] DEFAULT_SEVERITIES = new[] { 0, 1, 2, 3, 4 };
    public SyslogService(IDatabaseService database)
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
            SyslogServiceHelper.CalculateAlertSyslogs(_database, _devicesSyslogAlerts);
            _lastUpdate = DateTime.Now;
        }
    }
    private int[] LoadDesiredSeveritiesConfig()
    {
        int[] severities;
        var toSave = _database.GetConfigValue("DesiredSeverities");
        if (string.IsNullOrEmpty(toSave))
        {
            severities = DEFAULT_SEVERITIES;
        }
        else
        {
            var data = JsonSerializer.Deserialize<int[]>(toSave);
            if (data == null)
                severities = DEFAULT_SEVERITIES;
            else
                severities = data;
        }
        return severities;
    }
}