namespace NetDeviceManager.Database.Tables;

public class TagOnPhysicalDevice
{
    public Guid Id { get; set; }
    
    public Guid DeviceId { get; set; }
    public virtual PhysicalDevice Device { get; set; }
    
    public Guid TagId { get; set; }
    public virtual Tag Tag { get; set; }
}