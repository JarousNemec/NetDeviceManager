using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class DeviceService : IDeviceService
{
    private readonly ApplicationDbContext _database;
    private readonly IDatabaseService _databaseService;
    private readonly ISettingsService _settingsService;

    private readonly List<PhysicalDevice> _onlineDevices = [];
    private readonly List<PhysicalDevice> _offlineDevices = [];
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);

    public DeviceService(ApplicationDbContext database, IDatabaseService databaseService, ISettingsService settingsService)
    {
        _database = database;
        _databaseService = databaseService;
        _settingsService = settingsService;
    }
    
    public OperationResult AssignSensorToDevice(CorrectDataPattern model)
    {
        _databaseService.UpsertCorrectDataPattern(model);
        var relationship = new SnmpSensorInPhysicalDevice()
        {
            PhysicalDeviceId = model.PhysicalDeviceId,
            SnmpSensorId = model.SensorId
        };
        AddSnmpSensorToPhysicalDevice(relationship);

        var job = GetPhysicalDeviceSchedulerJob(model.PhysicalDeviceId);
        if (job != null) return new OperationResult() { IsSuccessful = false, Message = "Cannot create job!!!" };

        var newJob = new SchedulerJob();
        newJob.PhysicalDeviceId = model.PhysicalDeviceId;
        newJob.Type = SchedulerJobType.SNMPGET;
        newJob.Cron = _settingsService.GetSettings().ReportSensorInterval;
        _databaseService.AddSchedulerJob(newJob);

        return new OperationResult();
    }
    
    public OperationResult RemoveSensorFromDevice(SnmpSensorInPhysicalDevice relationShip)
    {
        var res = DeleteSnmpSensorInPhysicalDevice(relationShip.Id);
        if (!res.IsSuccessful)
        {
            return new OperationResult() { IsSuccessful = false, Message = "Bad id" };
        }

        var pattern = _databaseService.GetSpecificPattern(relationShip.PhysicalDeviceId, relationShip.SnmpSensorId);
        if (pattern == null)
        {
            return new OperationResult() { IsSuccessful = false, Message = "Bad pattern" };
        }

        _databaseService.DeleteCorrectDataPattern(pattern.Id);


        if (GetPhysicalDeviceSensorsCount(relationShip.PhysicalDeviceId) == 0)
        {
            _databaseService.DeleteDeviceSchedulerJob(relationShip.PhysicalDeviceId);
        }

        return new OperationResult();
    }

    public OperationResult UpdateIpAddressesAndDeviceRelations(List<string> ipAddresses, Guid deviceId)
    {
        var currentRelations = GetPhysicalDeviceIpAddressesRelations(deviceId);
        var toAdd = new List<PhysicalDeviceHasIpAddress>();
        var toRemove = new List<PhysicalDeviceHasIpAddress>();

        foreach (var ipAddress in ipAddresses.Select(v => v.Trim()))
        {
            if (currentRelations.All(x => x.IpAddress != ipAddress))
            {
                if (IPAddress.TryParse(ipAddress, out IPAddress? ip))
                    toAdd.Add(new PhysicalDeviceHasIpAddress()
                    {
                        IpAddress = ip.ToString(),
                        PhysicalDeviceId = deviceId
                    });
            }
        }

        foreach (var relation in currentRelations)
        {
            if (!ipAddresses.Contains(relation.IpAddress))
            {
                toRemove.Add(relation);
            }
        }

        try
        {
            foreach (var relation in toAdd)
            {
                AddPhysicalDeviceHasIpAddress(relation);
            }

            foreach (var relation in toRemove)
            {
                DeletePhysicalDeviceHasIpAddress(relation.Id);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return new OperationResult() { IsSuccessful = false, Message = e.Message };
        }


        return new OperationResult();
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
        // var devices = GetAllPhysicalDevices();
        online.Clear();
        offline.Clear();
        // var maxAge = TimeSpan.TicksPerMinute * 15;
        // foreach (var device in devices)
        // {
        //     var lastRecord = _snmpService.GetLastDeviceRecord(device.Id);
        //     if (lastRecord == null)
        //     {
        //         offline.Add(device);
        //         continue;
        //     }
        //
        //     if ((DateTime.Now - lastRecord.CapturedTime).Ticks > maxAge)
        //     {
        //         offline.Add(device);
        //     }
        //     else
        //     {
        //         online.Add(device);
        //     }
        // }

        //todo: move to device manager service
        //change method to pinging device to see if they are online
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