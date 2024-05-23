using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class PhysicalDevice
{
    public Guid Id { get; set; }
    [Required]public string Name { get; set; }
    public string? Description { get; set; }
    [Required]public string IpAddress { get; set; }
    public string? MacAddress { get; set; }
    
    [Required]public Guid DeviceId { get; set; }
    public virtual Device Device { get; set; }
    
    [Required]public Guid LoginProfileId { get; set; }
    public virtual LoginProfile LoginProfile { get; set; }
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SensorsInDevice { get; set; }
    
    public virtual IEnumerable<TagOnPhysicalDevice> TagsOnDevice { get; set; }
    
    public virtual IEnumerable<PhysicalDeviceHasPort> PortsInDevice { get; set; }
    
}