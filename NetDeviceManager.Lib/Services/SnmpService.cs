using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Snmp.Interfaces;
using NetDeviceManager.Lib.Snmp.Models;

namespace NetDeviceManager.Lib.Services;

public class SnmpService : ISnmpService
{
    private const int MESSANGER_GET_TIMEOUT = 10000;
    private readonly IDatabaseService _database;
    public SnmpService(IDatabaseService database)
    {
        _database = database;
    }

    public OperationResult CreateSnmpSensor(CreateSnmpSensorModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult AssignSensorToDevice(SnmpSensorInPhysicalDevice model)
    {
        throw new NotImplementedException();
    }

    public List<SnmpVariableModel>? GetSensorValue(SnmpSensor sensor, LoginProfile profile, PhysicalDevice device, Port port)
    {
        if (sensor.SnmpVersion != VersionCode.V3)
            return ReadSensorV1V2(sensor, device, port);
        if (string.IsNullOrEmpty(profile.AuthenticationPassword) || string.IsNullOrEmpty(profile.PrivacyPassword) || string.IsNullOrEmpty(profile.SecurityName))
            return null;
        return ReadSensorV3(sensor, profile, device, port);
    }

    private readonly List<Guid> _devicesSnmpAlerts = new();
    private int _alertCount = 0;
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);
    public int GetSnmpAlertsCount()
    {
        CheckTimelinessOfData();
        return _devicesSnmpAlerts.Count;
    }

    public int GetCurrentDeviceSnmpAlertsCount(Guid id)
    {
        CheckTimelinessOfData();
        return _devicesSnmpAlerts.Count(x => x == id);
    }

    public List<SnmpSensor> GetSensorsInDevice(Guid deviceId)
    {
        throw new NotImplementedException();
    }

    public SnmpSensor GetSensor(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpdateSnmpSensor(SnmpSensor model)
    {
        throw new NotImplementedException();
    }

    public OperationResult DeleteSnmpSensor(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationResult RemoveSensorFromDevice(SnmpSensorInPhysicalDevice model)
    {
        throw new NotImplementedException();
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
                    results.Add(new SnmpVariableModel(){DeviceId = device.Id, Index = i, SensorId = sensor.Id, Value = value});
                }
            }
            else
            {
                var value = SnmpGetV1V2(sensor, device, port, sensor.Oid);
                results.Add(new SnmpVariableModel(){DeviceId = device.Id, Index = 0, SensorId = sensor.Id, Value = value});
            }
        return results;
    }

    private string SnmpGetV1V2(SnmpSensor sensor, PhysicalDevice device, Port port, string oid)
    {
        var objectId = new ObjectIdentifier(oid);
        try
        {
            var result = Messenger.Get(sensor.SnmpVersion,
                new IPEndPoint(IPAddress.Parse(device.IpAddress), port.Number),
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

    private List<SnmpVariableModel> ReadSensorV3(SnmpSensor sensor, LoginProfile profile, PhysicalDevice device, Port port)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(device.IpAddress), port.Number);
        var results = new List<SnmpVariableModel>();
        if (sensor.IsMulti)
        {
            for (int i = (int)sensor.StartIndex!; i <= sensor.EndIndex; i++)
            {
                var orderOid = $"{sensor.Oid}{sensor.OidFilling}{i}";
                var value = SnmpGetV3(profile, device, endpoint, orderOid);
                results.Add(new SnmpVariableModel(){DeviceId = device.Id, Index = i, SensorId = sensor.Id, Value = value});
            }
        }
        else
        {
            var value = SnmpGetV3(profile, device, endpoint, sensor.Oid);
            results.Add(new SnmpVariableModel(){DeviceId = device.Id, Index = 0, SensorId = sensor.Id, Value = value});
        }
        return results;
    }

    private string SnmpGetV3(LoginProfile profile, PhysicalDevice device, IPEndPoint endpoint, string oid)
    {
        var objectId = new ObjectIdentifier(oid);
        try
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            ReportMessage report = discovery.GetResponse(MESSANGER_GET_TIMEOUT, endpoint);
            var auth = new SHA1AuthenticationProvider(new OctetString(profile.AuthenticationPassword));
            var priv = new DESPrivacyProvider(new OctetString(profile.PrivacyPassword), auth);

            GetRequestMessage request = new GetRequestMessage(
                VersionCode.V3, 
                Messenger.NextMessageId,
                Messenger.NextRequestId, 
                new OctetString(profile.Username),
                new List<Variable> { new Variable(objectId) }, 
                priv,
                Messenger.MaxMessageSize, report);
        
            ISnmpMessage reply = request.GetResponse(MESSANGER_GET_TIMEOUT, endpoint);
        
            if (reply.Pdu().ErrorStatus.ToInt32() != 0)
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(device.IpAddress),
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