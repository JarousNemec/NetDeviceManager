using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

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

    public Guid? AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice)
    {
        if (!_database.SnmpSensorsInPhysicalDevices.All(x =>
                x.PhysicalDeviceId != sensorInPhysicalDevice.PhysicalDeviceId && x.SnmpSensorId != sensorInPhysicalDevice.SnmpSensorId))
            return null;
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

    public Guid? AddCorrectDataPattern(CorrectDataPattern pattern)
    {
        if (!_database.CorrectDataPatterns.All(x =>
                x.PhysicalDeviceId != pattern.PhysicalDeviceId && x.SensorId != pattern.SensorId))
            return null;
        
        var id = GenerateGuid();
        pattern.Id = id;
        pattern.CapturedTime = DateTime.Now;
        _database.CorrectDataPatterns.Add(pattern);
        _database.SaveChanges();
        return id;
    }

    public void UpdatePhysicalDevice(PhysicalDevice model)
    {
        if (_database.PhysicalDevices.Any(x => x.Id == model.Id))
        {
            _database.PhysicalDevices.Update(model);
            _database.SaveChanges();
        }
    }

    public void UpdateSnmpSensor(SnmpSensor model)
    {
        if (_database.SnmpSensors.Any(x => x.Id == model.Id))
        {
            _database.SnmpSensors.Update(model);
            _database.SaveChanges();
        }
    }

    public List<Device> GetDevices()
    {
        return _database.Devices.ToList();
    }

    public List<LoginProfile> GetLoginProfiles()
    {
        return _database.LoginProfiles.ToList();
    }

    public List<SchedulerJob> GetSnmpReadJobs()
    {
        return _database.SchedulerJobs.Include(x => x.PhysicalDevice).ToList();
    }

    public List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId)
    {
        return _database.SnmpSensorsInPhysicalDevices
            .Where(x => x.PhysicalDeviceId == physicalDeviceId)
            .Include(x => x.PhysicalDevice)
            .Include(x => x.SnmpSensor)
            .ToList();
    }

    public List<PhysicalDeviceHasPort> GetPortInPhysicalDevices(Guid deviceId)
    {
        return _database.PhysicalDevicesHasPorts.Where(x => x.DeviceId == deviceId).Include(x => x.Port).ToList();
    }

    public LoginProfile? GetLoginProfile(Guid id)
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

    public bool IsAnySensorInDevice(Guid id)
    {
        return _database.SnmpSensorsInPhysicalDevices.Any(x => x.PhysicalDeviceId == id);
    }

    public List<SnmpSensorRecord> GetLastSnmpRecords(int count)
    {
        return _database.SnmpSensorRecords.Include(x => x.PhysicalDevice).Include(x => x.Sensor)
            .OrderByDescending(x => x.CapturedTime).Take(count).ToList();
    }

    public List<SyslogRecord> GetLastSyslogRecords(int count)
    {
        var data = _database.SyslogRecords.Include(x => x.PhysicalDevice)
            .OrderByDescending(x => x.ProcessedDate).Take(count).ToList();
        return data;
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
        return _database.SnmpSensorRecords.Where(x => x.PhysicalDeviceId == id).OrderByDescending(x => x.CapturedTime)
            .FirstOrDefault();
    }

    public List<PhysicalDevice> GetPhysicalDevices()
    {
        var data = _database.PhysicalDevices.Include(x => x.Device).ToList();
        return data;
    }

    public List<PhysicalDevice> GetCompletePhysicalDevices(Guid id)
    {
        return _database.PhysicalDevices.Where(x => x.Id == id)
            .Include(x => x.Device)
            .Include(x => x.LoginProfile)
            .Include(x => x.PortsInDevice)
            .Include(x => x.SensorsInDevice)
            .Include(x => x.TagsOnDevice)
            .ToList();
    }

    public List<CorrectDataPattern> GetPhysicalDevicesPatterns()
    {
        return _database.CorrectDataPatterns.Include(x =>x.PhysicalDevice).Include(x=>x.Sensor).ToList();
    }

    public List<Guid> GetSyslogsBySeverity(int severity)
    {
        return _database.SyslogRecords.Where(x => x.Severity == severity).Select(x => x.Id).ToList();
    }

    public List<Guid> GetSyslogs()
    {
        return _database.SyslogRecords.Select(x => x.Id).ToList();
    }

    public List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count)
    {
        IQueryable<SnmpSensorRecord> query = _database.SnmpSensorRecords.Include(x => x.PhysicalDevice)
            .Include(x => x.Sensor);
        if (!string.IsNullOrEmpty(model.DeviceName))
        {
            query = query.Where(x => x.PhysicalDevice.Name == model.DeviceName);
        }

        if (!string.IsNullOrEmpty(model.IpAddress))
        {
            query = query.Where(x => x.PhysicalDevice.IpAddress == model.IpAddress);
        }

        if (!string.IsNullOrEmpty(model.Oid))
        {
            query = query.Where(x => x.Sensor.Oid == model.Oid);
        }

        if (!string.IsNullOrEmpty(model.SensorName))
        {
            query = query.Where(x => x.Sensor.Name == model.SensorName);
        }

        return query.OrderByDescending(x =>x.CapturedTime).Take(count).ToList();
    }

    public List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count)
    {
        IQueryable<SyslogRecord> query = _database.SyslogRecords.Include(x => x.PhysicalDevice);
        if (!string.IsNullOrEmpty(model.DeviceName))
        {
            query = query.Where(x => x.PhysicalDevice.Name == model.DeviceName);
        }

        if (!string.IsNullOrEmpty(model.IpAddress))
        {
            query = query.Where(x => x.Ip == model.IpAddress);
        }

        if (model.Facility >= 0)
        {
            query = query.Where(x => x.Facility == model.Facility);
        }

        if (model.Severity >= 0)
        {
            query = query.Where(x => x.Severity == model.Severity);
        }

        return query.OrderByDescending(x =>x.ProcessedDate).Take(count).ToList();
    }

    public List<DeviceIcon> GetIcons()
    {
        return _database.DeviceIcons.ToList();
    }

    public List<Port> GetPortsInSystem()
    {
        return _database.Ports.ToList();
    }

    public List<SnmpSensor> GetSensors()
    {
        return _database.SnmpSensors.ToList();
    }

    public int GetSensorUsagesCount(Guid id)
    {
        return _database.SnmpSensorsInPhysicalDevices.Count(x => x.SnmpSensorId == id);
    }

    public CorrectDataPattern? GetSpecificPattern(Guid deviceId, Guid sensorId)
    {
        return _database.CorrectDataPatterns.FirstOrDefault(x =>
            x.PhysicalDeviceId == deviceId && x.SensorId == sensorId);
    }

    public OperationResult DeletePhysicalDevice(Guid id)
    {
        var device = _database.PhysicalDevices.FirstOrDefault(x => x.Id == id);
        if (device == null)
            return new OperationResult() { IsSuccessful = false, Message = "Unknown Id" };

        var tickets = _database.Tickets.Where(x => x.DeviceId == id);
        _database.Tickets.RemoveRange(tickets);
        var tags = _database.TagsOnPhysicalDevices.Where(x => x.DeviceId == id);
        _database.TagsOnPhysicalDevices.RemoveRange(tags);
        var sensorsInPd = _database.SnmpSensorsInPhysicalDevices.Where(x => x.PhysicalDeviceId == id);
        _database.SnmpSensorsInPhysicalDevices.RemoveRange(sensorsInPd);
        var jobs = _database.SchedulerJobs.Where(x => x.PhysicalDeviceId == id);
        _database.SchedulerJobs.RemoveRange(jobs);
        var pdHasPorts = _database.PhysicalDevicesHasPorts.Where(x => x.DeviceId == id);
        _database.PhysicalDevicesHasPorts.RemoveRange(pdHasPorts);
        var patterns = _database.CorrectDataPatterns.Where(x => x.PhysicalDeviceId == id);
        _database.CorrectDataPatterns.RemoveRange(patterns);

        _database.PhysicalDevices.Remove(device);
        _database.SaveChanges();
        return new OperationResult();
    }

    public OperationResult RemovePortFromDevice(Guid id)
    {
        var record = _database.PhysicalDevicesHasPorts.FirstOrDefault(x => x.Id == id);
        if (record != null)
        {
            _database.PhysicalDevicesHasPorts.Remove(record);
            _database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Cannot remove port" };
    }

    public OperationResult DeleteSnmpSensor(Guid id)
    {
        var sensor = _database.SnmpSensors.FirstOrDefault(x => x.Id == id);
        if (sensor == null)
        {
            return new OperationResult() { IsSuccessful = false, Message = "Unknown Id" };
        }

        var records = _database.SnmpSensorRecords.Where(x => x.SensorId == id);
        _database.SnmpSensorRecords.RemoveRange(records);

        var relationShips = _database.SnmpSensorsInPhysicalDevices.Where(x => x.SnmpSensorId == id);
        _database.SnmpSensorsInPhysicalDevices.RemoveRange(relationShips);

        _database.SnmpSensors.Remove(sensor);
        _database.SaveChanges();

        return new OperationResult();
    }

    public OperationResult DeleteSnmpSensorInPhysicalDevice(Guid id)
    {
        var relationship = _database.SnmpSensorsInPhysicalDevices.FirstOrDefault(x => x.Id == id);
        if (relationship == null)
            return new OperationResult() { IsSuccessful = false, Message = "Unknown id" };
        _database.SnmpSensorsInPhysicalDevices.Remove(relationship);
        _database.SaveChanges();
        return new OperationResult();
    }
    public OperationResult DeleteCorrectDataPattern(Guid id)
    {
        var pattern = _database.CorrectDataPatterns.FirstOrDefault(x => x.Id == id);
        if (pattern == null)
            return new OperationResult() { IsSuccessful = false, Message = "Unknown id" };
        _database.CorrectDataPatterns.Remove(pattern);
        _database.SaveChanges();
        return new OperationResult();
    }

    public bool AnyPhysicalDeviceWithIp(string ip)
    {
        return _database.PhysicalDevices.Any(x => x.IpAddress == ip);
    }

    public bool PortExists(Port port, out Guid id)
    {
        var existing = _database.Ports.FirstOrDefault(x => x.Number == port.Number && x.Protocol == port.Protocol);
        if (existing == null)
        {
            id = new Guid();
            return false;
        }

        id = existing.Id;
        return true;
    }

    public bool PortAddDeviceRelationExists(Guid portId, Guid deviceId, out Guid id)
    {
        var existing =
            _database.PhysicalDevicesHasPorts.FirstOrDefault(x => x.DeviceId == deviceId && x.PortId == portId);
        if (existing == null)
        {
            id = new Guid();
            return false;
        }

        id = existing.Id;
        return true;
    }
}