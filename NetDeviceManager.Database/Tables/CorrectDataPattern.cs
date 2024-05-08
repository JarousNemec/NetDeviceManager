namespace NetDeviceManager.Database.Tables;

public class CorrectDataPattern
{
    public Guid Id { get; set; }
    
    public string Data { get; set; }
    public DateTime CapturedTime { get; set; }

    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
    
    public Guid SensorId { get; set; }
    public virtual SnmpSensor Sensor { get; set; }
}