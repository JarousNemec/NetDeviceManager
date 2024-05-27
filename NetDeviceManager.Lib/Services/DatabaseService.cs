using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Identity;
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

    public Guid UpsertPort(Port port)
    {
        if (PortExists(port, out Guid id))
        {
            if (port.IsDefault)
                return id;
            var existingPort = _database.Ports.FirstOrDefault(x => x.Id == id);
            existingPort.IsDefault = port.IsDefault;
            _database.Ports.Update(port);
            _database.SaveChanges();
            return id;
        }
        else
        {
            id = GenerateGuid();

            port.Id = id;
            _database.Ports.Add(port);
            _database.SaveChanges();
            return id;
        }
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
                x.PhysicalDeviceId != sensorInPhysicalDevice.PhysicalDeviceId &&
                x.SnmpSensorId != sensorInPhysicalDevice.SnmpSensorId))
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

    public Guid? UpsertCorrectDataPattern(CorrectDataPattern pattern)
    {
        if (pattern.Id != null && pattern.Id != new Guid())
        {
            _database.CorrectDataPatterns.Update(pattern);
            _database.SaveChanges();
            return pattern.Id;
        }

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

    public void SetConfigValue(string key, string value)
    {
        var record = _database.Settings.FirstOrDefault(x => x.Key == key);
        if (record != null)
        {
            record.Value = value;
            _database.Settings.Update(record);
            _database.SaveChanges();
            return;
        }

        var newRecord = new Setting();
        newRecord.Id = GenerateGuid();
        newRecord.Key = key;
        newRecord.Value = value;
        _database.Settings.Add(newRecord);
        _database.SaveChanges();
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
        return _database.Devices.AsNoTracking().ToList();
    }

    public List<LoginProfile> GetLoginProfiles()
    {
        return _database.LoginProfiles.AsNoTracking().ToList();
    }

    public List<SchedulerJob> GetSchedulerJobs()
    {
        return _database.SchedulerJobs.AsNoTracking().Include(x => x.PhysicalDevice).ToList();
    }

    public SchedulerJob? GetPhysicalDeviceSchedulerJob(Guid id)
    {
        return _database.SchedulerJobs.AsNoTracking().Include(x => x.PhysicalDevice).FirstOrDefault(x => x.PhysicalDeviceId == id);
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
        return _database.PhysicalDevicesHasPorts.AsNoTracking().Where(x => x.DeviceId == deviceId).Include(x => x.Port).ToList();
    }

    public LoginProfile? GetLoginProfile(Guid id)
    {
        return _database.LoginProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public Guid GetSnmpSensorInPhysicalDeviceId(Guid sensorId, Guid deviceId)
    {
        return _database.SnmpSensorsInPhysicalDevices.AsNoTracking().FirstOrDefault(x =>
            x.PhysicalDeviceId == deviceId && x.SnmpSensorId == sensorId).Id;
    }

    public int GetRecordsCount()
    {
        return _database.SnmpSensorRecords.AsNoTracking().Count();
    }

    public int GetDeviceSensorsCount(Guid id)
    {
        return _database.SnmpSensorsInPhysicalDevices.AsNoTracking().Count(x => x.PhysicalDeviceId == id);
    }

    public bool IsAnySensorInDevice(Guid id)
    {
        return _database.SnmpSensorsInPhysicalDevices.AsNoTracking().Any(x => x.PhysicalDeviceId == id);
    }

    public List<SnmpSensorRecord> GetLastSnmpRecords(int count)
    {
        return _database.SnmpSensorRecords.AsNoTracking().Include(x => x.PhysicalDevice).Include(x => x.Sensor)
            .OrderByDescending(x => x.CapturedTime).Take(count).ToList();
    }

    public List<SyslogRecord> GetLastSyslogRecords(int count)
    {
        var data = _database.SyslogRecords.AsNoTracking().Include(x => x.PhysicalDevice)
            .OrderByDescending(x => x.ProcessedDate).Take(count).ToList();
        return data;
    }

    public PhysicalDevice? GetPhysicalDeviceByIp(string ip)
    {
        return _database.PhysicalDevices.AsNoTracking().FirstOrDefault(x => x.IpAddress == ip);
    }

    public string? GetConfigValue(string key)
    {
        return _database.Settings.AsNoTracking().FirstOrDefault(x => x.Key == key)?.Value;
    }

    public SnmpSensorRecord? GetLastDeviceRecord(Guid id)
    {
        return _database.SnmpSensorRecords.AsNoTracking().Where(x => x.PhysicalDeviceId == id).OrderByDescending(x => x.CapturedTime)
            .FirstOrDefault();
    }

    public List<PhysicalDevice> GetPhysicalDevices()
    {
        var data = _database.PhysicalDevices.AsNoTracking().Include(x => x.Device).ToList();
        return data;
    }

    public List<PhysicalDevice> GetCompletePhysicalDevices(Guid id)
    {
        return _database.PhysicalDevices.AsNoTracking().Where(x => x.Id == id)
            .Include(x => x.Device)
            .Include(x => x.LoginProfile)
            .Include(x => x.PortsInDevice)
            .Include(x => x.SensorsInDevice)
            .Include(x => x.TagsOnDevice)
            .ToList();
    }

    public List<CorrectDataPattern> GetPhysicalDevicesPatterns()
    {
        return _database.CorrectDataPatterns.AsNoTracking().Include(x => x.PhysicalDevice).Include(x => x.Sensor).ToList();
    }

    public List<Guid> GetSyslogsBySeverity(int severity)
    {
        return _database.SyslogRecords.AsNoTracking().Where(x => x.Severity == severity).Select(x => x.Id).ToList();
    }

    public List<Guid> GetSyslogs()
    {
        return _database.SyslogRecords.AsNoTracking().Select(x => x.Id).ToList();
    }

    public List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count)
    {
        IQueryable<SnmpSensorRecord> query = _database.SnmpSensorRecords.AsNoTracking().Include(x => x.PhysicalDevice)
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

        return query.OrderByDescending(x => x.CapturedTime).Take(count).ToList();
    }

    public List<SyslogRecord> GetSyslogRecordsWithFilter(SyslogRecordFilterModel model, int count)
    {
        IQueryable<SyslogRecord> query = _database.SyslogRecords.AsNoTracking().Include(x => x.PhysicalDevice);
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

        return query.OrderByDescending(x => x.ProcessedDate).Take(count).ToList();
    }

    public List<DeviceIcon> GetIcons()
    {
        return _database.DeviceIcons.AsNoTracking().ToList();
    }

    public List<Port> GetPortsInSystem()
    {
        return _database.Ports.AsNoTracking().ToList();
    }

    public List<Port> GetDefaultPorts()
    {
        return _database.Ports.AsNoTracking().Where(x => x.IsDefault).ToList();
    }

    public List<SnmpSensor> GetSensors()
    {
        return _database.SnmpSensors.AsNoTracking().ToList();
    }

    public int GetSensorUsagesCount(Guid id)
    {
        return _database.SnmpSensorsInPhysicalDevices.AsNoTracking().Count(x => x.SnmpSensorId == id);
    }

    public CorrectDataPattern? GetSpecificPattern(Guid deviceId, Guid sensorId)
    {
        return _database.CorrectDataPatterns.AsNoTracking().FirstOrDefault(x =>
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
        var record = _database.PhysicalDevicesHasPorts.FirstOrDefault(x => x.PortId == id);
        if (record != null)
        {
            _database.PhysicalDevicesHasPorts.Remove(record);
            _database.SaveChanges();

            if (_database.PhysicalDevicesHasPorts.Count(x => x.PortId == record.PortId) == 0)
            {
                var port = _database.Ports.FirstOrDefault(x => x.Id == record.PortId);
                if (port != null)
                {
                    _database.Ports.Remove(port);
                    _database.SaveChanges();
                }
            }

            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Cannot remove port" };
    }

    public OperationResult RemoveDefaultPort(Guid id)
    {
        var port = _database.Ports.FirstOrDefault(x => x.Id == id);
        if (port != null)
        {
            if (_database.PhysicalDevicesHasPorts.Count(x => x.PortId == port.Id) == 0)
            {
                    _database.Ports.Remove(port);
                    _database.SaveChanges();
            }
            else
            {
                port.IsDefault = false;
                _database.Ports.Update(port);
                _database.SaveChanges();
            }
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Unknown id" };
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

    public OperationResult DeleteDeviceSchedulerJob(Guid id)
    {
        var job = _database.SchedulerJobs.FirstOrDefault(x => x.PhysicalDeviceId == id);
        if (job != null)
        {
            _database.SchedulerJobs.Remove(job);
            _database.SaveChanges();
        }

        return new OperationResult();
    }

    public OperationResult DeleteUser(string id)
    {
        var user = _database.Users.FirstOrDefault(x => x.Id == id);
        if (user != null)
        {
            _database.Users.Remove(user);
            _database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Unknown id" };
    }

    public bool AnyPhysicalDeviceWithIp(string ip)
    {
        return _database.PhysicalDevices.AsNoTracking().Any(x => x.IpAddress == ip);
    }

    public bool PortExists(Port port, out Guid id)
    {
        var existing = _database.Ports.AsNoTracking().FirstOrDefault(x => x.Number == port.Number && x.Protocol == port.Protocol);
        if (existing == null)
        {
            id = new Guid();
            return false;
        }

        id = existing.Id;
        return true;
    }

    public bool PortAndDeviceRelationExists(Guid portId, Guid deviceId, out Guid id)
    {
        var existing =
            _database.PhysicalDevicesHasPorts.AsNoTracking().FirstOrDefault(x => x.DeviceId == deviceId && x.PortId == portId);
        if (existing == null)
        {
            id = new Guid();
            return false;
        }

        id = existing.Id;
        return true;
    }
}