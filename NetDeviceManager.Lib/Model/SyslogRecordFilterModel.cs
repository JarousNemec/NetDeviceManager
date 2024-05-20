namespace NetDeviceManager.Database.Models;

public class SyslogRecordFilterModel
{
    public string DeviceName { get; set; }
    public string IpAddress { get; set; }
    public int Facility { get; set; } = -1;
    public int Severity { get; set; } = -1;
}