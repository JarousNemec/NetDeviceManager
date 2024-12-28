using NetDeviceManager.Database.Identity;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IDatabaseService
{
    Guid AddSnmpRecord(SnmpSensorRecord record);

    Guid AddSnmpSensor(SnmpSensor sensor);

    Guid AddSchedulerJob(SchedulerJob job);

    Guid AddSyslogRecord(SyslogRecord record);

    Guid? UpsertCorrectDataPattern(CorrectDataPattern pattern);

    void SetConfigValue(string key, string value);
    void UpdateSnmpSensor(SnmpSensor model);
    List<SchedulerJob> GetSchedulerJobs();
    

    List<SnmpSensorRecord> GetLastSnmpRecords(int count);

    List<SyslogRecord> GetLastSyslogRecords(int count);

    string? GetConfigValue(string key);

    SnmpSensorRecord? GetLastDeviceRecord(Guid id);
    List<Guid> GetSyslogs();

    List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count = -1);

    List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count = -1);
    List<SyslogRecord> GetSyslogRecordsWithUnknownSource(int count = -1);
    
    List<SnmpSensor> GetSensors();

    int GetSensorUsagesCount(Guid id);

    CorrectDataPattern? GetSpecificPattern(Guid deviceId, Guid sensorId);
    
    OperationResult DeleteSnmpSensor(Guid id);
    OperationResult DeleteCorrectDataPattern(Guid id);

    OperationResult DeleteDeviceSchedulerJob(Guid id);

    OperationResult DeleteUser(string id);
    bool IsAnySensorInDevice(Guid id);
}