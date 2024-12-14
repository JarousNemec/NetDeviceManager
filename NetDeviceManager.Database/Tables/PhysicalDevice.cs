using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class PhysicalDevice
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? MacAddress { get; set; }

    public Guid? IconId { get; set; }
    public virtual DeviceIcon? Icon { get; set; }
    
    public string Platform { get; set; }
    public string? Version { get; set; }
    public string? Capabilities { get; set; }
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> Sensors { get; set; }
    
    public virtual IEnumerable<TagOnPhysicalDevice> Tags { get; set; }
    
    public virtual IEnumerable<PhysicalDeviceHasPort> Ports { get; set; }
    
    public virtual IEnumerable<LoginProfileToPhysicalDevice> LoginProfiles { get; set; }
    
    public virtual IEnumerable<PhysicalDeviceHasIpAddress> IpAddresses { get; set; }
    
}