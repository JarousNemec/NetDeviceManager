namespace NetDeviceManager.Database.Tables;

public class PhysicalDeviceReadInterval
{
    public Guid Id { get; set; }

    public string SchedulerCron { get; set; }

    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}