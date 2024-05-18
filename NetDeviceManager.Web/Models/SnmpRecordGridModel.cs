namespace NetDeviceManager.Web.Models;

public class SnmpRecordGridModel
{
    public Guid Id { get; set; }
    public string Device { get; set; }
    public string Sensor { get; set; }
    public string Data { get; set; }
}