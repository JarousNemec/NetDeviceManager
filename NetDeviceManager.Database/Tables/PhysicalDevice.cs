namespace NetDeviceManager.Database.Tables;

public class PhysicalDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string IpAddress { get; set; }
    public string? MacAddress { get; set; }
    public int Port { get; set; }
    
    public Guid DeviceId { get; set; }
    public virtual Device Device { get; set; }
    
    public int LoginType { get; set; } = 1;
    public Guid? CredentialsId { get; set; }
    public virtual CredentialsData? Credentials { get; set; }
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SensorsInDevice { get; set; }
    
    public virtual IEnumerable<TagOnPhysicalDevice> TagsOnDevice { get; set; }
    
}