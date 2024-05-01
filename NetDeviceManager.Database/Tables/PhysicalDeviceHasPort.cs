namespace NetDeviceManager.Database.Tables;

public class PhysicalDeviceHasPort
{
    public Guid Id { get; set; }

    public Guid PortId { get; set; }
    public virtual Port Port { get; set; }
    
    public Guid DeviceId { get; set; }
    public virtual PhysicalDevice Device { get; set; }
}