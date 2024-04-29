using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.ScheduledSnmpAgent.Models;

public class ReadDeviceSensorJobModel
{
    private List<SnmpSensorInPhysicalDevice> _sensors;
    private PhysicalDevice _device;
    private string? _id;
}