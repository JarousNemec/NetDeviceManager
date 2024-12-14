using System.Net;
using System.Text.Json;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Snmp.Models;

namespace NetDeviceManager.Lib.Services;

public class SnmpService : ISnmpService
{
    private const int MESSANGER_GET_TIMEOUT = 10000;
    private readonly IDatabaseService _database;
    private readonly SettingsService _settingsService;

    public SnmpService(IDatabaseService database, SettingsService settingsService)
    {
        _database = database;
        _settingsService = settingsService;
    }

    public OperationResult UpsertSnmpSensor(SnmpSensor model, out Guid id)
    {
        if (model.Id != new Guid())
        {
            id = model.Id;
            _database.UpdateSnmpSensor(model);
            return new OperationResult();
        }

        id = _database.AddSnmpSensor(model);
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
        _database.AddSnmpSensorToPhysicalDevice(relationship);

        var job = _database.GetPhysicalDeviceSchedulerJob(model.PhysicalDeviceId);
        if (job != null) return new OperationResult(){IsSuccessful = false, Message = "Cannot create job!!!"};
        
        var newJob = new SchedulerJob();
        newJob.PhysicalDeviceId = model.PhysicalDeviceId;
        newJob.Type = SchedulerJobType.SNMPGET;
        newJob.Cron = _settingsService.GetSettings().ReportSensorInterval;
        _database.AddSchedulerJob(newJob);

        return new OperationResult();
    }

    public string? GetSensorValue(SnmpSensor sensor, List<LoginProfile> profiles, PhysicalDevice device, Port port)
    {
        List<SnmpVariableModel> freshData;
        
        //todo: lepsi souseni logovacish profilu 
        var profile = profiles.FirstOrDefault(x => !string.IsNullOrEmpty(x.SnmpAuthenticationPassword) &&
                                                   !string.IsNullOrEmpty(x.SnmpPrivacyPassword) && !string.IsNullOrEmpty(x.SnmpSecurityName) && !string.IsNullOrEmpty(x.SnmpUsername));
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

    public List<SnmpSensor> GetSensorsInDevice(Guid deviceId)
    {
        throw new NotImplementedException();
    }

    public SnmpSensor GetSensor(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<SnmpAlertModel> GetAlerts()
    {
        return _devicesSnmpAlerts;
    }

    public OperationResult UpdateSnmpSensor(SnmpSensor model)
    {
        throw new NotImplementedException();
    }

    public void RemoveAlert(Guid id)
    {
        var alert = _devicesSnmpAlerts.FirstOrDefault(x => x.Id == id);
        if (alert != null)
            _devicesSnmpAlerts.Remove(alert);
    }

    public OperationResult RemoveSensorFromDevice(SnmpSensorInPhysicalDevice relationShip)
    {
        var res = _database.DeleteSnmpSensorInPhysicalDevice(relationShip.Id);
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

        
        if (_database.GetDeviceSensorsCount(relationShip.PhysicalDeviceId) == 0)
        {
            _database.DeleteDeviceSchedulerJob(relationShip.PhysicalDeviceId);
        }
        
        return new OperationResult();
    }

    private void CheckTimelinessOfData()
    {
        if ((DateTime.Now.Ticks - _lastUpdate.Ticks) > (TimeSpan.TicksPerMinute * 5))
        {
            SnmpServiceHelper.CalculateSnmpAlerts(_database, _devicesSnmpAlerts);
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
        if(ipAddress == null)
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
        if(ipAddress == null)
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
        if(string.IsNullOrEmpty(profile.SnmpPrivacyPassword) || string.IsNullOrEmpty(profile.SnmpUsername) || string.IsNullOrEmpty(profile.SnmpAuthenticationPassword) || string.IsNullOrEmpty(profile.SnmpSecurityName))
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