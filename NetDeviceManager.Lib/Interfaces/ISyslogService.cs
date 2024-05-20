namespace NetDeviceManager.Lib.Interfaces;

public interface ISyslogService
{
    public int GetSyslogAlertCount();
    public int GetCurrentDeviceSyslogAlertsCount(Guid id);
}