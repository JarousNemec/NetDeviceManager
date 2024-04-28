using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database.Identity;
using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<CredentialsData> CredentialsDatas { get; set; }
    
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceIcon> DeviceIcons { get; set; }
    
    public DbSet<PhysicalDevice> PhysicalDevices { get; set; }
    public DbSet<SnmpSensor> SnmpSensors { get; set; }
    public DbSet<PhysicalDeviceReadJob> PhysicalDevicesReadJobs { get; set; }
    public DbSet<SnmpSensorInPhysicalDevice> SensorsInPhysicalDevices { get; set; }
    
    public DbSet<SnmpSensorRecord> SnmpSensorRecords { get; set; }
    public DbSet<SyslogRecord> SyslogRecords { get; set; }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TagOnPhysicalDevice> TagsOnPhysicalDevices { get; set; }
}