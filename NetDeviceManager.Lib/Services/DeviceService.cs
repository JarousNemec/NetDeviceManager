using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class DeviceService : IDeviceService
{
    private readonly ApplicationDbContext _database;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPortService _portService;
    private readonly IDatabaseService _databaseService;

    private readonly List<PhysicalDevice> _onlineDevices = new List<PhysicalDevice>();
    private readonly List<PhysicalDevice> _offlineDevices = new List<PhysicalDevice>();
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);

    public DeviceService(IDatabaseService databaseService, ApplicationDbContext database,
        IFileStorageService fileStorageService, IPortService portService)
    {
        _database = database;
        _fileStorageService = fileStorageService;
        _portService = portService;
        _databaseService = databaseService;
    }

    public OperationResult UpsertPhysicalDevice(PhysicalDevice model, out Guid id)
    {
        if (model.Id != default)
        {
            UpdatePhysicalDevice(model);
            id = model.Id;
            return new OperationResult();
        }

        id = AddPhysicalDevice(model);
        return new OperationResult();
    }

    public OperationResult AddPortToDevice(Port model, Guid deviceId)
    {
        var portId = _portService.UpsertPort(model);

        if (!_portService.PortAndDeviceRelationExists(portId, deviceId))
        {
            var relationship = new PhysicalDeviceHasPort()
            {
                DeviceId = deviceId,
                PortId = portId
            };
            _portService.AddPortToPhysicalDevice(relationship);
        }

        return new OperationResult();
    }

    public int GetOnlineDevicesCount()
    {
        CheckTimelinessOfData();
        return _onlineDevices.Count;
    }

    public List<PhysicalDevice> GetOnlineDevices()
    {
        CheckTimelinessOfData();
        return _onlineDevices;
    }

    public int GetOfflineDevicesCount()
    {
        CheckTimelinessOfData();
        return _offlineDevices.Count;
    }

    public List<PhysicalDevice> GetOfflineDevices()
    {
        CheckTimelinessOfData();
        return _offlineDevices;
    }

    private void CheckTimelinessOfData()
    {
        if ((DateTime.Now.Ticks - _lastUpdate.Ticks) > (TimeSpan.TicksPerMinute * 5))
        {
            CalculateOnlineOfflineDevices(_onlineDevices, _offlineDevices);
            _lastUpdate = DateTime.Now;
        }
    }

    public void CalculateOnlineOfflineDevices(List<PhysicalDevice> online, List<PhysicalDevice> offline)
    {
        var devices = GetAllPhysicalDevices();
        online.Clear();
        offline.Clear();
        var maxAge = TimeSpan.TicksPerMinute * 15;
        foreach (var device in devices)
        {
            var lastRecord = _databaseService.GetLastDeviceRecord(device.Id);
            if (lastRecord == null)
            {
                offline.Add(device);
                continue;
            }

            if ((DateTime.Now - lastRecord.CapturedTime).Ticks > maxAge)
            {
                offline.Add(device);
            }
            else
            {
                online.Add(device);
            }
        }
    }

    public Guid AddPhysicalDevice(PhysicalDevice physicalDevice)
    {
        var id = DatabaseUtil.GenerateId();
        physicalDevice.Id = id;
        _database.PhysicalDevices.Add(physicalDevice);
        _database.SaveChanges();
        return id;
    }

    public Guid? AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice)
    {
        if (!_database.SnmpSensorsInPhysicalDevices.All(x =>
                x.PhysicalDeviceId != sensorInPhysicalDevice.PhysicalDeviceId &&
                x.SnmpSensorId != sensorInPhysicalDevice.SnmpSensorId))
            return null;
        var id = DatabaseUtil.GenerateId();
        sensorInPhysicalDevice.Id = id;
        _database.SnmpSensorsInPhysicalDevices.Add(sensorInPhysicalDevice);
        _database.SaveChanges();
        return id;
    }

    public Guid AddPhysicalDeviceHasIpAddress(PhysicalDeviceHasIpAddress physicalDeviceHasIpAddress)
    {
        var id = DatabaseUtil.GenerateId();
        physicalDeviceHasIpAddress.Id = id;
        _database.PhysicalDevicesHaveIpAddresses.Add(physicalDeviceHasIpAddress);
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

    public SchedulerJob? GetPhysicalDeviceSchedulerJob(Guid id)
    {
        return _database.SchedulerJobs.AsNoTracking().Include(x => x.PhysicalDevice)
            .FirstOrDefault(x => x.PhysicalDeviceId == id);
    }

    public List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId)
    {
        return _database.SnmpSensorsInPhysicalDevices
            .Where(x => x.PhysicalDeviceId == physicalDeviceId)
            .Include(x => x.PhysicalDevice)
            .Include(x => x.SnmpSensor)
            .ToList();
    }

    public PhysicalDevice? GetPhysicalDeviceByIp(string ip)
    {
        return _database.PhysicalDevices.AsNoTracking().Include(x => x.IpAddresses)
            .FirstOrDefault(x => x.IpAddresses.Any(y => y.IpAddress == ip));
    }

    public List<PhysicalDevice> GetAllPhysicalDevices()
    {
        var data = _database.PhysicalDevices.AsNoTracking().ToList();
        return data;
    }

    public List<PhysicalDevice> GetPhysicalDevicesWithIpAddresses()
    {
        var data = _database.PhysicalDevices.AsNoTracking().Include(x => x.IpAddresses).ToList();
        return data;
    }

    public List<PhysicalDeviceHasIpAddress> GetPhysicalDeviceIpAddressesRelations(Guid deviceId)
    {
        return _database.PhysicalDevicesHaveIpAddresses
            .AsNoTracking()
            .Where(x => x.PhysicalDeviceId == deviceId)
            .ToList();
    }

    public List<CorrectDataPattern> GetPhysicalDevicesPatterns()
    {
        return _database.CorrectDataPatterns.AsNoTracking().Include(x => x.PhysicalDevice).Include(x => x.Sensor)
            .ToList();
    }

    public List<string> GetPhysicalDeviceIpAddresses(Guid deviceId)
    {
        return _database.PhysicalDevicesHaveIpAddresses.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Select(x => x.IpAddress).ToList();
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
        var pdHasPorts = _database.PhysicalDevicesHavePorts.Where(x => x.DeviceId == id);
        _database.PhysicalDevicesHavePorts.RemoveRange(pdHasPorts);
        var patterns = _database.CorrectDataPatterns.Where(x => x.PhysicalDeviceId == id);
        _database.CorrectDataPatterns.RemoveRange(patterns);

        _database.PhysicalDevices.Remove(device);
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

    public OperationResult DeleteSyslogsOfDevice(Guid? physicalDeviceId)
    {
        foreach (var record in _database.SyslogRecords.Where(x => x.PhysicalDeviceId == physicalDeviceId))
        {
            _database.SyslogRecords.Remove(record);
        }

        _database.SaveChanges();
        return new OperationResult();
    }

    public OperationResult DeletePhysicalDeviceHasIpAddress(Guid id)
    {
        var item = _database.PhysicalDevicesHaveIpAddresses.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            _database.PhysicalDevicesHaveIpAddresses.Remove(item);
            _database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Cannot remove ip address device relation" };
    }

    public int GetPhysicalDeviceSensorsCount(Guid id)
    {
        return _database.SnmpSensorsInPhysicalDevices.AsNoTracking().Count(x => x.PhysicalDeviceId == id);
    }
}