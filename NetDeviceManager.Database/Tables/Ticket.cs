using Microsoft.AspNetCore.Identity;

namespace NetDeviceManager.Database.Tables;

public class Ticket
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public long Created { get; set; }
    public long Updated { get; set; }
    
    public Guid? DeviceId { get; set; }
    public virtual PhysicalDevice? Device { get; set; }

    public string UserId { get; set; }
    public virtual IdentityUser User { get; set; }
}