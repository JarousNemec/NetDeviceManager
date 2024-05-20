namespace NetDeviceManager.Database.Models;

public class SnmpRecordFilterModel
{
    public string DeviceName { get; set; }
    public string IpAddress { get; set; }
    public string SensorName { get; set; }
    public string Oid { get; set; }
}