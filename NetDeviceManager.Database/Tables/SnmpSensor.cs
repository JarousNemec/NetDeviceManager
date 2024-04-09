namespace NetDeviceManager.Database.Tables;

public class SnmpSensor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Oid { get; set; }
    public string SnmpVersion { get; set; }

    public Guid CommunityId { get; set; }
    public virtual Community Community { get; set; }
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SnmpSensorInDevices { get; set; }
}