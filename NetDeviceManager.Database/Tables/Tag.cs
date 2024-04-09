using System.Drawing;

namespace NetDeviceManager.Database.Tables;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Color Color { get; set; }
}