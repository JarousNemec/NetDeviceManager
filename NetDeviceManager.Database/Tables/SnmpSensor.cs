using System.ComponentModel.DataAnnotations;
using Lextm.SharpSnmpLib;

namespace NetDeviceManager.Database.Tables;

public class SnmpSensor
{
    public Guid Id { get; set; }
    [Required] public string Name { get; set; }
    public string? Description { get; set; }
    [Required] public string Oid { get; set; }
    [Required] public VersionCode SnmpVersion { get; set; }
    [Required] public string CommunityString { get; set; }

    [Required] public bool IsMulti { get; set; } = false;
    public string OidFilling { get; set; } = ".";

    public int StartIndex { get; set; } = 0;
    public int EndIndex { get; set; } = 0;

    public virtual IEnumerable<SnmpSensorInPhysicalDevice> SnmpSensorInDevices { get; set; }
}