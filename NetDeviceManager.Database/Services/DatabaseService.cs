using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Database.Services;

public class DatabaseService : IDatabaseService
{
    private readonly ApplicationDbContext _database;

    public DatabaseService(ApplicationDbContext database)
    {
        _database = database;
    }
    private Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    public Guid AddSnmpRecord(SnmpSensorRecord record)
    {
        var id = GenerateGuid();
        record.Id = id;
        _database.SnmpSensorRecords.Add(record);
        _database.SaveChanges();
        return id;
    }

    public Guid AddDeviceIcon(DeviceIcon icon)
    {
        var id = GenerateGuid();
        icon.Id = id;
        _database.DeviceIcons.Add(icon);
        _database.SaveChanges();
        return id;
    }

    public Guid AddDevice(Device device)
    {
        var id = GenerateGuid();
        device.Id = id;
        _database.Devices.Add(device);
        _database.SaveChanges();
        return id;
    }

    public Guid AddOidIntegerLabel(OidIntegerLabel label)
    {
        var id = GenerateGuid();
        label.Id = id;
        _database.OidIntegerLabels.Add(label);
        _database.SaveChanges();
        return id;
    }

    public Guid AddLoginProfile(LoginProfile profile)
    {
        var id = GenerateGuid();
        profile.Id = id;
        _database.LoginProfiles.Add(profile);
        _database.SaveChanges();
        return id;
    }

    public Guid AddPhysicalDevice(PhysicalDevice physicalDevice)
    {
        var id = GenerateGuid();
        physicalDevice.Id = id;
        _database.PhysicalDevices.Add(physicalDevice);
        _database.SaveChanges();
        return id;
    }

    public Guid AddPort(Port port)
    {
        var id = GenerateGuid();
        port.Id = id;
        _database.Ports.Add(port);
        _database.SaveChanges();
        return id;
    }

    public Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort)
    {
        var id = GenerateGuid();
        physicalDeviceHasPort.Id = id;
        _database.PhysicalDevicesHasPorts.Add(physicalDeviceHasPort);
        _database.SaveChanges();
        return id;
    }

    public Guid AddSnmpSensor(SnmpSensor sensor)
    {
        var id = GenerateGuid();
        sensor.Id = id;
        _database.SnmpSensors.Add(sensor);
        _database.SaveChanges();
        return id;
    }

    public Guid AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice)
    {
        var id = GenerateGuid();
        sensorInPhysicalDevice.Id = id;
        _database.SnmpSensorsInPhysicalDevices.Add(sensorInPhysicalDevice);
        _database.SaveChanges();
        return id;
    }

    public Guid AddSchedulerJob(SchedulerJob job)
    {
        var id = GenerateGuid();
        job.Id = id;
        _database.SchedulerJobs.Add(job);
        _database.SaveChanges();
        return id;
    }

    public Guid AddSyslogRecord(SyslogRecord record)
    {
        var id = GenerateGuid();
        record.Id = id;
        _database.SyslogRecords.Add(record);
        _database.SaveChanges();
        return id;
    }

    public Guid AddTag(Tag tag)
    {
        var id = GenerateGuid();
        tag.Id = id;
        _database.Tags.Add(tag);
        _database.SaveChanges();
        return id;
    }

    public Guid AddTicket(Ticket ticket)
    {
        var id = GenerateGuid();
        ticket.Id = id;
        _database.Tickets.Add(ticket);
        _database.SaveChanges();
        return id;
    }

    public Guid AddTagOnPhysicalDevice(TagOnPhysicalDevice tagOnPhysicalDevice)
    {
        var id = GenerateGuid();
        tagOnPhysicalDevice.Id = id;
        _database.TagsOnPhysicalDevices.Add(tagOnPhysicalDevice);
        _database.SaveChanges();
        return id;
    }

    public List<SchedulerJob> GetSnmpReadJobs()
    {
        return _database.SchedulerJobs.Include(x => x.PhysicalDevice).ToList();
    }

    public List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId)
    {
        return _database.SnmpSensorsInPhysicalDevices
            .Where(x => x.PhysicalDeviceId == physicalDeviceId)
            .Include(x => x.SnmpSensor)
            .ToList();
    }

    public List<PhysicalDeviceHasPort> GetPortInPhysicalDevices(Guid deviceId)
    {
        return _database.PhysicalDevicesHasPorts.Where(x => x.DeviceId == deviceId).Include(x => x.Port).ToList();
    }

    public LoginProfile GetPhysicalDeviceLoginProfile(Guid id)
    {
        return _database.LoginProfiles.FirstOrDefault(x => x.Id == id);
    }

    public Guid GetSnmpSensorInPhysicalDeviceId(Guid sensorId, Guid deviceId)
    {
        return _database.SnmpSensorsInPhysicalDevices.FirstOrDefault(x =>
            x.PhysicalDeviceId == deviceId && x.SnmpSensorId == sensorId).Id;
    }

    public int GetRecordsCount()
    {
        return _database.SnmpSensorRecords.Count();
    }

    public PhysicalDevice? GetPhysicalDeviceByIp(string ip)
    {
        return _database.PhysicalDevices.FirstOrDefault(x => x.IpAddress == ip);
    }

    public string? GetConfigValue(string key)
    {
        return _database.Settings.FirstOrDefault(x => x.Key == key)?.Value;
    }

    public SnmpSensorRecord? GetLastDeviceRecord(Guid id)
    {
        return _database.SnmpSensorRecords.Where(x => x.PhysicalDeviceId == id).OrderByDescending(x => x.CapturedTime).FirstOrDefault();
    }

    public List<PhysicalDevice> GetPhysicalDevices()
    {
        return _database.PhysicalDevices.ToList();
    }

    public List<CorrectDataPattern> GetPhysicalDevicesPatterns()
    {
        return _database.CorrectDataPatterns.ToList();
    }

    public List<Guid> GetSyslogsBySeverity(int severity)
    {
        return _database.SyslogRecords.Where(x => x.Severity == severity).Select(x => x.Id).ToList();
    }

    public List<Guid> GetSyslogs()
    {
        return _database.SyslogRecords.Select(x => x.Id).ToList();
    }
}