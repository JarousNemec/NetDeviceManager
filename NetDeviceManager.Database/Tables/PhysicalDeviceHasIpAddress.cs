namespace NetDeviceManager.Database.Tables;

public class PhysicalDeviceHasIpAddress
{
    public Guid Id { get; set; }
    
    public string IpAddress { get; set; } = null!;

    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; } = null!;

    public override string ToString()
    {
        return IpAddress;
    }
}