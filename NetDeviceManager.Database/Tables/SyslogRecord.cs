using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Database.Tables;

public class SyslogRecord
{
    public Guid Id { get; set; }
    public string CompletMessage { get; set; }
    public SyslogFacility Facility { get; set; }
    public SyslogSeverity Severity { get; set; }
    public string Message { get; set; }
    public DateTime ProcessedDate { get; set; }
    public DateTime CreationDate { get; set; }

    public string Ip { get; set; }
    public Guid? PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}