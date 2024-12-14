using System.Reflection.Metadata.Ecma335;

namespace NetDeviceManager.Database.Tables;

public class DeviceIcon
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Url { get; set; }
}