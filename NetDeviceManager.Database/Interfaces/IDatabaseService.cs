using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Database.Interfaces;

public interface IDatabaseService
{
    Guid AddSnmpRecord(SnmpSensorRecord record);
   
    Guid AddDeviceIcon(DeviceIcon icon);
    Guid AddDevice(Device device);

    Guid AddOidIntegerLabel(OidIntegerLabel label);

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
    
    List<SchedulerJob> GetSnmpReadJobs();
    List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId);
    List<PhysicalDeviceHasPort> GetPortInPhysicalDevices(Guid deviceId);

    LoginProfile GetPhysicalDeviceLoginProfile(Guid id);
    Guid GetSnmpSensorInPhysicalDeviceId(Guid sensor, Guid device);

    int GetRecordsCount();


}