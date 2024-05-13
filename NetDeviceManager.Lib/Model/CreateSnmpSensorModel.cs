using Lextm.SharpSnmpLib;

namespace NetDeviceManager.Lib.Model;

public class CreateSnmpSensorModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Oid { get; set; }
    public VersionCode SnmpVersion { get; set; }
    public string CommunityString { get; set; }

    public bool IsMulti { get; set; } = false;
    public string OidFilling { get; set; } = ".";

    public int StartIndex { get; set; } = 0;
    public int EndIndex { get; set; } = 0;
}