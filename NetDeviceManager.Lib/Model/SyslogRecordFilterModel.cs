using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Lib.Model;

public class SyslogRecordFilterModel
{
    public string DeviceName { get; set; } = string.Empty;
    public string IpAddresses { get; set; } = string.Empty;
    public SyslogFacility Facility { get; set; } = SyslogFacility.Undefined;
    public SyslogSeverity Severity { get; set; } = SyslogSeverity.Undefined;
}