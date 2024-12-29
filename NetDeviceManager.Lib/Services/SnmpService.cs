using System.Net;
using System.Text.Json;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Snmp.Models;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class SnmpService : ISnmpService
{
    private const int MESSANGER_GET_TIMEOUT = 10000;
    private readonly IDatabaseService _database;
    private readonly ApplicationDbContext _dbContext;
    private readonly SettingsService _settingsService;
    private const int DEFAULT_PORT = 161;

    private readonly IDeviceService _deviceService;

    public SnmpService(IDeviceService deviceService, IDatabaseService database, SettingsService settingsService,
        ApplicationDbContext dbContext)
    {
        _deviceService = deviceService;
        _database = database;
        _settingsService = settingsService;
        _dbContext = dbContext;
    }

    public OperationResult UpsertSnmpSensor(SnmpSensor model, out Guid id)
    {
        if (model.Id != new Guid())
        {
            id = model.Id;
            UpdateSnmpSensor(model);
            return new OperationResult();
        }

        id = AddSnmpSensor(model);
        return new OperationResult();
    }

    public OperationResult AssignSensorToDevice(CorrectDataPattern model)
    {
        _database.UpsertCorrectDataPattern(model);
        var relationship = new SnmpSensorInPhysicalDevice()
        {
            PhysicalDeviceId = model.PhysicalDeviceId,
            SnmpSensorId = model.SensorId
        };
        _deviceService.AddSnmpSensorToPhysicalDevice(relationship);

        var job = _deviceService.GetPhysicalDeviceSchedulerJob(model.PhysicalDeviceId);
        if (job != null) return new OperationResult() { IsSuccessful = false, Message = "Cannot create job!!!" };

        var newJob = new SchedulerJob();
        newJob.PhysicalDeviceId = model.PhysicalDeviceId;
        newJob.Type = SchedulerJobType.SNMPGET;
        newJob.Cron = _settingsService.GetSettings().ReportSensorInterval;
        _database.AddSchedulerJob(newJob);

        return new OperationResult();
    }

    public string? GetSensorValue(SnmpSensor sensor, List<LoginProfile> profiles, PhysicalDevice device, Port? port)
    {
        if (port == null)
            port = new Port() { Number = DEFAULT_PORT };

        List<SnmpVariableModel> freshData;

        //todo: lepsi souseni logovacish profilu 
        var profile = profiles.FirstOrDefault(x => !string.IsNullOrEmpty(x.SnmpAuthenticationPassword) &&
                                                   !string.IsNullOrEmpty(x.SnmpPrivacyPassword) &&
                                                   !string.IsNullOrEmpty(x.SnmpSecurityName) &&
                                                   !string.IsNullOrEmpty(x.SnmpUsername));
        if (sensor.SnmpVersion != VersionCode.V3)
            freshData = ReadSensorV1V2(sensor, device, port);
        else if (profile == null)
            //todo zalogovat se nenalezen logovaci profil pro dany snmp sensor
            return null;
        else
        {
            freshData = ReadSensorV3(sensor, profile, device, port);
        }

        var data = new string[sensor.EndIndex - sensor.StartIndex + 1];
        foreach (var item in freshData)
        {
            data[item.Index] = item.Value;
        }

        return JsonSerializer.Serialize(data);
    }

    private readonly List<SnmpAlertModel> _devicesSnmpAlerts = new();
    private int _alertCount = 0;
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);

    public int GetSnmpAlertsCount()
    {
        CheckTimelinessOfData();
        return _devicesSnmpAlerts.Count;
    }

    public int GetDeviceSnmpAlertsCount(Guid id)
    {
        CheckTimelinessOfData();
        return _devicesSnmpAlerts.Count(x => x.Device.Id == id);
    }


    public int GetSensorSnmpAlertsCount(Guid id)
    {
        CheckTimelinessOfData();
        return _devicesSnmpAlerts.Count(x => x.Sensor.Id == id);
    }

    public bool IsSensorOfDeviceOk(Guid deviceId, Guid sensorId)
    {
        return _devicesSnmpAlerts.All(x => x.Device.Id != deviceId && x.Sensor.Id != sensorId);
    }

    public List<SnmpAlertModel> GetAlerts()
    {
        return _devicesSnmpAlerts;
    }

    public void RemoveAlert(Guid id)
    {
        var alert = _devicesSnmpAlerts.FirstOrDefault(x => x.Id == id);
        if (alert != null)
            _devicesSnmpAlerts.Remove(alert);
    }

    public OperationResult RemoveSensorFromDevice(SnmpSensorInPhysicalDevice relationShip)
    {
        var res = _deviceService.DeleteSnmpSensorInPhysicalDevice(relationShip.Id);
        if (!res.IsSuccessful)
        {
            return new OperationResult() { IsSuccessful = false, Message = "Bad id" };
        }

        var pattern = _database.GetSpecificPattern(relationShip.PhysicalDeviceId, relationShip.SnmpSensorId);
        if (pattern == null)
        {
            return new OperationResult() { IsSuccessful = false, Message = "Bad pattern" };
        }

        _database.DeleteCorrectDataPattern(pattern.Id);


        if (_deviceService.GetPhysicalDeviceSensorsCount(relationShip.PhysicalDeviceId) == 0)
        {
            _database.DeleteDeviceSchedulerJob(relationShip.PhysicalDeviceId);
        }

        return new OperationResult();
    }

    public Guid AddSnmpRecord(SnmpSensorRecord record)
    {
        var id = DatabaseUtil.GenerateId();
        record.Id = id;
        _dbContext.SnmpSensorRecords.Add(record);
        _dbContext.SaveChanges();
        return id;
    }

    public void UpdateSnmpSensor(SnmpSensor model)
    {
        if (_dbContext.SnmpSensors.Any(x => x.Id == model.Id))
        {
            _dbContext.SnmpSensors.Update(model);
            _dbContext.SaveChanges();
        }
    }

    public Guid AddSnmpSensor(SnmpSensor sensor)
    {
        var id = DatabaseUtil.GenerateId();
        sensor.Id = id;
        _dbContext.SnmpSensors.Add(sensor);
        _dbContext.SaveChanges();
        return id;
    }

    public List<SnmpSensorRecord> GetLastSnmpRecords(int count)
    {
        return _dbContext.SnmpSensorRecords.AsNoTracking().Include(x => x.PhysicalDevice).Include(x => x.Sensor)
            .OrderByDescending(x => x.CapturedTime).Take(count).ToList();
    }

    public SnmpSensorRecord? GetLastDeviceRecord(Guid id)
    {
        return _dbContext.SnmpSensorRecords.AsNoTracking().Where(x => x.PhysicalDeviceId == id)
            .OrderByDescending(x => x.CapturedTime)
            .FirstOrDefault();
    }

    public List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count = -1)
    {
        IQueryable<SnmpSensorRecord> query = _dbContext.SnmpSensorRecords.AsNoTracking().Include(x => x.PhysicalDevice)
            .Include(x => x.Sensor);
        if (!string.IsNullOrEmpty(model.DeviceName))
        {
            query = query.Where(x => x.PhysicalDevice.Name == model.DeviceName);
        }

        if (!string.IsNullOrEmpty(model.Oid))
        {
            query = query.Where(x => x.Sensor.Oid == model.Oid);
        }

        if (!string.IsNullOrEmpty(model.SensorName))
        {
            query = query.Where(x => x.Sensor.Name == model.SensorName);
        }

        if (count == -1)
            return query.OrderByDescending(x => x.CapturedTime).ToList();
        return query.OrderByDescending(x => x.CapturedTime).Take(count).ToList();
    }

    public List<SnmpSensor> GetAllSensors()
    {
        return _dbContext.SnmpSensors.AsNoTracking().ToList();
    }

    public int GetSensorUsagesCount(Guid id)
    {
        return _dbContext.SnmpSensorsInPhysicalDevices.AsNoTracking().Count(x => x.SnmpSensorId == id);
    }

    public OperationResult DeleteSnmpSensor(Guid id)
    {
        var sensor = _dbContext.SnmpSensors.FirstOrDefault(x => x.Id == id);
        if (sensor == null)
        {
            return new OperationResult() { IsSuccessful = false, Message = "Unknown Id" };
        }

        var records = _dbContext.SnmpSensorRecords.Where(x => x.SensorId == id);
        _dbContext.SnmpSensorRecords.RemoveRange(records);

        var relationShips = _dbContext.SnmpSensorsInPhysicalDevices.Where(x => x.SnmpSensorId == id);
        _dbContext.SnmpSensorsInPhysicalDevices.RemoveRange(relationShips);

        _dbContext.SnmpSensors.Remove(sensor);
        _dbContext.SaveChanges();

        return new OperationResult();
    }

    public bool IsAnySensorInDevice(Guid id)
    {
        return _dbContext.SnmpSensorsInPhysicalDevices.AsNoTracking().Any(x => x.PhysicalDeviceId == id);
    }

    private void CheckTimelinessOfData()
    {
        if ((DateTime.Now.Ticks - _lastUpdate.Ticks) > (TimeSpan.TicksPerMinute * 5))
        {
            SnmpServiceHelper.CalculateSnmpAlerts(this,_deviceService, _devicesSnmpAlerts);
            _lastUpdate = DateTime.Now;
        }
    }


    private List<SnmpVariableModel> ReadSensorV1V2(SnmpSensor sensor, PhysicalDevice device, Port port)
    {
        var results = new List<SnmpVariableModel>();
        if (sensor.IsMulti)
        {
            for (int i = sensor.StartIndex!; i <= sensor.EndIndex; i++)
            {
                var orderOid = $"{sensor.Oid}{sensor.OidFilling}{i}";
                var value = SnmpGetV1V2(sensor, device, port, orderOid);
                results.Add(new SnmpVariableModel() { Index = i, Value = value });
            }
        }
        else
        {
            var value = SnmpGetV1V2(sensor, device, port, sensor.Oid);
            results.Add(new SnmpVariableModel() { Index = 0, Value = value });
        }

        return results;
    }

    private string SnmpGetV1V2(SnmpSensor sensor, PhysicalDevice device, Port port, string oid)
    {
        string? ipAddress = device.IpAddresses.First().ToString();
        if (ipAddress == null)
            return "-1";

        var objectId = new ObjectIdentifier(oid);
        try
        {
            var result = Messenger.Get(sensor.SnmpVersion,
                new IPEndPoint(IPAddress.Parse(ipAddress), port.Number),
                new OctetString(sensor.CommunityString),
                new List<Variable>
                    { new(objectId) }, MESSANGER_GET_TIMEOUT)[0];
            return result.Data.ToString();
        }
        catch (Exception e)
        {
            Console.Out.WriteLine(e.Message);
            return "-1";
        }
    }

    private List<SnmpVariableModel> ReadSensorV3(SnmpSensor sensor, LoginProfile profile, PhysicalDevice device,
        Port port)
    {
        var results = new List<SnmpVariableModel>();

        string? ipAddress = device.IpAddresses.First().ToString();
        if (ipAddress == null)
            return results;

        var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port.Number);

        if (sensor.IsMulti)
        {
            for (int i = (int)sensor.StartIndex!; i <= sensor.EndIndex; i++)
            {
                var orderOid = $"{sensor.Oid}{sensor.OidFilling}{i}";
                var value = SnmpGetV3(profile, device, endpoint, orderOid);
                results.Add(new SnmpVariableModel() { Index = i, Value = value });
            }
        }
        else
        {
            var value = SnmpGetV3(profile, device, endpoint, sensor.Oid);
            results.Add(new SnmpVariableModel() { Index = 0, Value = value });
        }

        return results;
    }

    private string SnmpGetV3(LoginProfile profile, PhysicalDevice device, IPEndPoint endpoint, string oid)
    {
        if (string.IsNullOrEmpty(profile.SnmpPrivacyPassword) || string.IsNullOrEmpty(profile.SnmpUsername) ||
            string.IsNullOrEmpty(profile.SnmpAuthenticationPassword) || string.IsNullOrEmpty(profile.SnmpSecurityName))
            return "-1";
        var objectId = new ObjectIdentifier(oid);
        try
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            ReportMessage report = discovery.GetResponse(MESSANGER_GET_TIMEOUT, endpoint);
            var auth = new SHA1AuthenticationProvider(new OctetString(profile.SnmpAuthenticationPassword));
            var priv = new DESPrivacyProvider(new OctetString(profile.SnmpPrivacyPassword), auth);

            GetRequestMessage request = new GetRequestMessage(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                new OctetString(profile.SnmpUsername),
                new List<Variable> { new Variable(objectId) },
                priv,
                Messenger.MaxMessageSize, report);

            ISnmpMessage reply = request.GetResponse(MESSANGER_GET_TIMEOUT, endpoint);
            //todo logovat co se tu deje treba chyba v authentikaci atd.
            if (reply.Pdu().ErrorStatus.ToInt32() != 0)
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(endpoint.Address.ToString()),
                    reply);
            }

            var result = reply.Pdu().Variables[0];
            return result.Data.ToString();
        }
        catch (Exception e)
        {
            Console.Out.WriteLine(e.Message);
            return "-1";
        }
    }
}