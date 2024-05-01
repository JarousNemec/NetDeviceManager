using Lextm.SharpSnmpLib;

namespace NetDeviceManager.Database.Tables;

public class SnmpSensor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Oid { get; set; }
    public VersionCode SnmpVersion { get; set; }
    public string CommunityString { get; set; }
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SnmpSensorInDevices { get; set; }
}