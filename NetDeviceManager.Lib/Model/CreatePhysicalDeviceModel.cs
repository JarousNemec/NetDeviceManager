namespace NetDeviceManager.Lib.Model;

public class CreatePhysicalDeviceModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string IpAddress { get; set; }
    public string? MacAddress { get; set; }
    
    public Guid DeviceId { get; set; }
    public Guid LoginProfileId { get; set; }
}