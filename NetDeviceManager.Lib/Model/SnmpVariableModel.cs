namespace NetDeviceManager.Lib.Snmp.Models;

public class SnmpVariableModel
{
    public Guid SensorId { get; set; }
    public Guid DeviceId { get; set; }
    public int Index { get; set; }
    public string Value { get; set; }
}