using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.ScheduledSnmpAgent.Interfaces;

public interface IDatabaseService
{
    void InsertNewSnmpRecord(Guid id, string value, long capturedTime, Guid sensorInPhysicalDeviceId);
    List<PhysicalDeviceReadJob> GetSnmpReadJobs();
    List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId);
}