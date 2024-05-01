using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.ScheduledSnmpAgent.Interfaces;

public interface IDatabaseService
{
    void InsertNewSnmpRecord(Guid id, string value, long capturedTime, Guid sensorInPhysicalDeviceId);
    List<SchedulerJob> GetSnmpReadJobs();
    List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId);
    List<PhysicalDeviceHasPort> GetPortInPhysicalDevices(Guid deviceId);

    LoginProfile GetPhysicalDeviceLoginProfile(Guid id);
}