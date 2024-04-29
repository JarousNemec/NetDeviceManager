namespace NetDeviceManager.Database.Tables;

public class SnmpSensorRecord
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public long CapturedTime { get; set; }

    public Guid SensorInPhysicalDeviceId { get; set; }
    public virtual SnmpSensorInPhysicalDevice SensorInPhysicalDevice { get; set; }
}