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

    public List<PhysicalDeviceReadJob> GetSnmpReadJobs()
    {
        return database.PhysicalDevicesReadJobs.Include(x => x.PhysicalDevice).ToList();
    }

    public List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId)
    {
        return database.SensorsInPhysicalDevices
            .Where(x => x.PhysicalDeviceId == physicalDeviceId)
            .Include(x => x.SnmpSensor)
            .Include(x => x.SnmpSensor.Community)
            .ToList();
    }
}