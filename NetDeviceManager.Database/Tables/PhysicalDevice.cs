namespace NetDeviceManager.Database.Tables;

public class PhysicalDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string IpAddress { get; set; }
    public string? MacAddress { get; set; }
    
    public Guid DeviceId { get; set; }
    public virtual Device Device { get; set; }
    
    public Guid LoginProfileId { get; set; }
    public virtual LoginProfile LoginProfile { get; set; }
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SensorsInDevice { get; set; }
    
    public virtual IEnumerable<TagOnPhysicalDevice> TagsOnDevice { get; set; }
    
    public virtual IEnumerable<PhysicalDeviceHasPort> PortsInDevice { get; set; }
    
}