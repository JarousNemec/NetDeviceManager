namespace NetDeviceManager.Database.Tables;

public class SnmpSensorInPhysicalDevice
{
    public Guid Id { get; set; }
    
    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
    
    public Guid SnmpSensorId { get; set; }
    public virtual SnmpSensor SnmpSensor { get; set; }
}