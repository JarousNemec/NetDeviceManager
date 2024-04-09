namespace NetDeviceManager.Database.Tables;

public class SyslogRecord
{
    public Guid Id { get; set; }
    
    public string Message { get; set; }
    public string Priority { get; set; }
    public string? Version { get; set; }
    public string Hostname { get; set; }
    public string? Application { get; set; }
    public string? ProcessId { get; set; }
    public string? MessageId { get; set; }
    public long Timestamp { get; set; }

    public Guid? PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}