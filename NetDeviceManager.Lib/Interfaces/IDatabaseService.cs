using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IDatabaseService
{
    #region Create

    Guid AddSnmpRecord(SnmpSensorRecord record);

    Guid AddDeviceIcon(DeviceIcon icon);
    Guid AddDevice(Device device);

    Guid AddLoginProfile(LoginProfile profile);

    Guid AddPhysicalDevice(PhysicalDevice physicalDevice);

    Guid AddPort(Port port);

    Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort);

    Guid AddSnmpSensor(SnmpSensor sensor);
    Guid AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice);

    Guid AddSchedulerJob(SchedulerJob job);

    Guid AddSyslogRecord(SyslogRecord record);
    Guid AddTag(Tag tag);

    Guid AddTicket(Ticket ticket);

    Guid AddTagOnPhysicalDevice(TagOnPhysicalDevice tagOnPhysicalDevice);

    #endregion

    #region Update

    void UpdatePhysicalDevice(PhysicalDevice model);
    void UpdateSnmpSensor(SnmpSensor model);

    #endregion

    #region Read

    List<Device> GetDevices();
    List<LoginProfile> GetLoginProfiles();
    List<SchedulerJob> GetSnmpReadJobs();
    List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId);
    List<PhysicalDeviceHasPort> GetPortInPhysicalDevices(Guid deviceId);

    LoginProfile GetPhysicalDeviceLoginProfile(Guid id);
    Guid GetSnmpSensorInPhysicalDeviceId(Guid sensor, Guid device);

    int GetRecordsCount();

    List<SnmpSensorRecord> GetLastSnmpRecords(int count);

    List<SyslogRecord> GetLastSyslogRecords(int count);

    PhysicalDevice? GetPhysicalDeviceByIp(string ip);

    string? GetConfigValue(string key);

    SnmpSensorRecord? GetLastDeviceRecord(Guid id);

    List<PhysicalDevice> GetPhysicalDevices();
    List<PhysicalDevice> GetCompletePhysicalDevices(Guid id);

    List<CorrectDataPattern> GetPhysicalDevicesPatterns();

    List<Guid> GetSyslogsBySeverity(int severity);
    List<Guid> GetSyslogs();

    List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count);

    List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count);

    List<DeviceIcon> GetIcons();

    List<Port> GetPortsInSystem();

    List<SnmpSensor> GetSensors();

    int GetSensorUsagesCount(Guid id);

    #endregion

    #region Delete

    OperationResult DeletePhysicalDevice(Guid id);
    OperationResult RemovePortFromDevice(Guid id);

    OperationResult DeleteSnmpSensor(Guid id);

    #endregion

    #region Special

    bool AnyPhysicalDeviceWithIp(string ip);
    bool PortExists(Port port, out Guid id);

    bool PortAddDeviceRelationExists(Guid portId, Guid deviceId, out Guid id);
    #endregion
}