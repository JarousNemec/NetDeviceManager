using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Database.Services;

public class DatabaseService(ApplicationDbContext database) : IDatabaseService
{
    private Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    public Guid AddSnmpRecord(SnmpSensorRecord record)
    {
        var id = GenerateGuid();
        record.Id = id;
        database.SnmpSensorRecords.Add(record);
        database.SaveChanges();
        return id;
    }

    public Guid AddDeviceIcon(DeviceIcon icon)
    {
        var id = GenerateGuid();
        icon.Id = id;
        database.DeviceIcons.Add(icon);
        database.SaveChanges();
        return id;
    }

    public Guid AddDevice(Device device)
    {
        var id = GenerateGuid();
        device.Id = id;
        database.Devices.Add(device);
        database.SaveChanges();
        return id;
    }

    public Guid AddOidIntegerLabel(OidIntegerLabel label)
    {
        var id = GenerateGuid();
        label.Id = id;
        database.OidIntegerLabels.Add(label);
        database.SaveChanges();
        return id;
    }

    public Guid AddLoginProfile(LoginProfile profile)
    {
        var id = GenerateGuid();
        profile.Id = id;
        database.LoginProfiles.Add(profile);
        database.SaveChanges();
        return id;
    }

    public Guid AddPhysicalDevice(PhysicalDevice physicalDevice)
    {
        var id = GenerateGuid();
        physicalDevice.Id = id;
        database.PhysicalDevices.Add(physicalDevice);
        database.SaveChanges();
        return id;
    }

    public Guid AddPort(Port port)
    {
        var id = GenerateGuid();
        port.Id = id;
        database.Ports.Add(port);
        database.SaveChanges();
        return id;
    }

    public Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort)
    {
        var id = GenerateGuid();
        physicalDeviceHasPort.Id = id;
        database.PhysicalDevicesHasPorts.Add(physicalDeviceHasPort);
        database.SaveChanges();
        return id;
    }

    public Guid AddSnmpSensor(SnmpSensor sensor)
    {
        var id = GenerateGuid();
        sensor.Id = id;
        database.SnmpSensors.Add(sensor);
        database.SaveChanges();
        return id;
    }

    public Guid AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice)
    {
        var id = GenerateGuid();
        sensorInPhysicalDevice.Id = id;
        database.SnmpSensorsInPhysicalDevices.Add(sensorInPhysicalDevice);
        database.SaveChanges();
        return id;
    }

    public Guid AddSchedulerJob(SchedulerJob job)
    {
        var id = GenerateGuid();
        job.Id = id;
        database.SchedulerJobs.Add(job);
        database.SaveChanges();
        return id;
    }

    public Guid AddSyslogRecord(SyslogRecord record)
    {
        var id = GenerateGuid();
        record.Id = id;
        database.SyslogRecords.Add(record);
        database.SaveChanges();
        return id;
    }

    public Guid AddTag(Tag tag)
    {
        var id = GenerateGuid();
        tag.Id = id;
        database.Tags.Add(tag);
        database.SaveChanges();
        return id;
    }

    public Guid AddTicket(Ticket ticket)
    {
        var id = GenerateGuid();
        ticket.Id = id;
        database.Tickets.Add(ticket);
        database.SaveChanges();
        return id;
    }

    public Guid AddTagOnPhysicalDevice(TagOnPhysicalDevice tagOnPhysicalDevice)
    {
        var id = GenerateGuid();
        tagOnPhysicalDevice.Id = id;
        database.TagsOnPhysicalDevices.Add(tagOnPhysicalDevice);
        database.SaveChanges();
        return id;
    }

    public List<SchedulerJob> GetSnmpReadJobs()
    {
        return database.SchedulerJobs.Include(x => x.PhysicalDevice).ToList();
    }

    public List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId)
    {
        return database.SnmpSensorsInPhysicalDevices
            .Where(x => x.PhysicalDeviceId == physicalDeviceId)
            .Include(x => x.SnmpSensor)
            .ToList();
    }

    public List<PhysicalDeviceHasPort> GetPortInPhysicalDevices(Guid deviceId)
    {
        return database.PhysicalDevicesHasPorts.Where(x => x.DeviceId == deviceId).Include(x => x.Port).ToList();
    }

    public LoginProfile GetPhysicalDeviceLoginProfile(Guid id)
    {
        return database.LoginProfiles.FirstOrDefault(x => x.Id == id);
    }

    public Guid GetSnmpSensorInPhysicalDeviceId(Guid sensorId, Guid deviceId)
    {
        return database.SnmpSensorsInPhysicalDevices.FirstOrDefault(x =>
            x.PhysicalDeviceId == deviceId && x.SnmpSensorId == sensorId).Id;
    }

    public int GetRecordsCount()
    {
        return database.SnmpSensorRecords.Count();
    }

    public PhysicalDevice? GetPhysicalDeviceByIp(string ip)
    {
        return database.PhysicalDevices.FirstOrDefault(x => x.IpAddress == ip);
    }

    public string? GetConfigValue(string key)
    {
        return database.Settings.FirstOrDefault(x => x.Key == key)?.Value;
    }
}