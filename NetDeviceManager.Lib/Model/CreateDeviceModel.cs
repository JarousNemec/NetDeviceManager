using Microsoft.AspNetCore.Http;

namespace NetDeviceManager.Lib.Model;

public class CreateDeviceModel
{
    public string Name { get; set; }
    public string Model { get; set; }
    public string? Description { get; set; }
    public string Brand { get; set; }
    public Guid IconId { get; set; }
}