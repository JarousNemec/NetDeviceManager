namespace NetDeviceManager.Database.Tables;

public class LoginProfileToPhysicalDevice
{
    public Guid Id { get; set; }
    
    public Guid LoginProfileId { get; set; }
    public virtual LoginProfile LoginProfile { get; set; }

    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}