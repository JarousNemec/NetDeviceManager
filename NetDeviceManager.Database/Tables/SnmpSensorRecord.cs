using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class SnmpSensorRecord
{
    public Guid Id { get; set; }
    
    public string Data { get; set; }
    public DateTime CapturedTime { get; set; }

    public Guid SensorInPhysicalDeviceId { get; set; }
    public virtual SnmpSensorInPhysicalDevice SensorInPhysicalDevice { get; set; }
}