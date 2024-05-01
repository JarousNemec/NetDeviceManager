using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.ScheduledSnmpAgent.Interfaces;

namespace NetDeviceManager.ScheduledSnmpAgent.Services;

public class DatabaseService(ApplicationDbContext database) : IDatabaseService
{
    public async void InsertNewSnmpRecord(Guid id, string value, long capturedTime, Guid sensorInPhysicalDeviceId)
    {
        var record = new SnmpSensorRecord()
        {
            Id = id,
            Value = value,
            CapturedTime = capturedTime,
            SensorInPhysicalDeviceId = sensorInPhysicalDeviceId
        };
        database.SnmpSensorRecords.Add(record);
        await database.SaveChangesAsync();
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
}