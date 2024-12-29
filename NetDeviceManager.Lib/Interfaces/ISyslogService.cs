using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface ISyslogService
{
    public int GetSyslogAlertCount();
    public int GetCurrentDeviceSyslogAlertsCount(Guid id);
    List<SyslogRecord> GetLastSyslogRecords(int count);
    
    Guid AddSyslogRecord(SyslogRecord record);
    List<Guid> GetAllSyslogs();

    List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count = -1);
    List<SyslogRecord> GetSyslogRecordsWithUnknownSource(int count = -1);
}