using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Database.Tables;

public class SchedulerJob
{
    public Guid Id { get; set; }

    public string Cron { get; set; }
    public SchedulerJobType Type { get; set; }
    public Guid PhysicalDeviceId { get; set; }
    public virtual PhysicalDevice PhysicalDevice { get; set; }
}