using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Lib.Model;

public class SyslogRecordFilterModel
{
    public string DeviceName { get; set; }
    public string IpAddress { get; set; }
    public SyslogFacility Facility { get; set; } = SyslogFacility.Undefined;
    public SyslogSeverity Severity { get; set; } = SyslogSeverity.Undefined;
}