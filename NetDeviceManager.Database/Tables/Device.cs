namespace NetDeviceManager.Database.Tables;

public class Device
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string? Description { get; set; }
    public string Brand { get; set; }
    
    public Guid? IconId { get; set; }
    public virtual DeviceIcon? Icon { get; set; }
}