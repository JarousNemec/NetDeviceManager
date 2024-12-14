using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database.Identity;
using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    public DbSet<LoginProfile> LoginProfiles { get; set; }
    public DbSet<DeviceIcon> DeviceIcons { get; set; }
    
    public DbSet<PhysicalDevice> PhysicalDevices { get; set; }
    public DbSet<SnmpSensor> SnmpSensors { get; set; }
    public DbSet<SchedulerJob> SchedulerJobs { get; set; }
    public DbSet<SnmpSensorInPhysicalDevice> SnmpSensorsInPhysicalDevices { get; set; }
    
    public DbSet<SnmpSensorRecord> SnmpSensorRecords { get; set; }
    public DbSet<SyslogRecord> SyslogRecords { get; set; }
    
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TagOnPhysicalDevice> TagsOnPhysicalDevices { get; set; }
    
    public DbSet<Port> Ports { get; set; }
    public DbSet<PhysicalDeviceHasPort> PhysicalDevicesHavePorts { get; set; }
    public DbSet<CorrectDataPattern> CorrectDataPatterns { get; set; }

    public DbSet<Setting> Settings { get; set; }
    
    public DbSet<LoginProfileToPhysicalDevice> LoginProfilesToPhysicalDevices { get; set; }
    public DbSet<PhysicalDeviceHasIpAddress> PhysicalDevicesHaveIpAddresses { get; set; }
    
    
}