namespace NetDeviceManager.Database.Tables;

public class PhysicalDeviceReadInterval
{
    public Guid Id { get; set; }
    
    public long SensorsReadInterval { get; set; } = 12000000000;
    public long LastSensorsRead { get; set; } = 0;

    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}