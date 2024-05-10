namespace NetDeviceManager.Database.Tables;

public class SyslogRecord
{
    public Guid Id { get; set; }
    public string CompletMessage { get; set; }
    public int Facility { get; set; }
    public int Severity { get; set; }
    public string Message { get; set; }
    public DateTime ProcessedDate { get; set; }

    public Guid? PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}