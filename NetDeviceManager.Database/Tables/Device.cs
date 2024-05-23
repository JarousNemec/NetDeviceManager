using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class Device
{
    public Guid Id { get; set; }
    [Required]public string Name { get; set; }
    [Required]public string Model { get; set; }
    public string? Description { get; set; }
    [Required]public string Brand { get; set; }
    
    public Guid? IconId { get; set; }
    public virtual DeviceIcon? Icon { get; set; }
}