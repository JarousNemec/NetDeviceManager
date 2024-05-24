using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Lib.Model;

public class SnmpAlertModel
{
    public Guid Id { get; set; }
    public PhysicalDevice Device { get; set; }
    public SnmpSensor Sensor { get; set; }
    public string Expected { get; set; }
    public string Current { get; set; }
    
}