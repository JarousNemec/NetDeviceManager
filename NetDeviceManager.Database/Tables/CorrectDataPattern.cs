using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class CorrectDataPattern
{
    public Guid Id { get; set; }
    
    public string Data { get; set; }
    public DateTime CapturedTime { get; set; }

    [Required] public bool HasToleration { get; set; } = false;
    public int Toleration { get; set; }

    [Required]public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
    
    [Required]public Guid SensorId { get; set; }
    public virtual SnmpSensor Sensor { get; set; }
}