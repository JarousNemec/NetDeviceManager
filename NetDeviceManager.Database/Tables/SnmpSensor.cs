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

    public bool IsMulti { get; set; } = false;

    public int? StartIndex { get; set; } = 0;
    public int? EndIndex { get; set; } = 0;
    
    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SnmpSensorInDevices { get; set; }
}