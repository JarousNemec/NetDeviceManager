using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Snmp.Interfaces;
using NetDeviceManager.Lib.Snmp.Utils;
using NetDeviceManager.ScheduledSnmpAgent.Interfaces;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;

namespace NetDeviceManager.ScheduledSnmpAgent.Jobs;

public class ReadDeviceSensorsJob : IJob
{
    private List<SnmpSensorInPhysicalDevice> _sensors;
    private int _port;
    private LoginProfile _login;
    private PhysicalDevice _device;
    private string? _id;
    private readonly IDatabaseService _database;
    private readonly ISnmpService _snmpService;

    public ReadDeviceSensorsJob(IDatabaseService database, ISnmpService snmpService)
    {
        _database = database;
        _snmpService = snmpService;
    }
    
    public Task Execute(IJobExecutionContext context)
    {
        InitializeJob(context);

        foreach (var sensor in _sensors)
        {
            //todo: add auth to db and everywhere else
            var snmpVersion = sensor.SnmpSensor.SnmpVersion;
            
            var results = _snmpService.GetSensorValue(snmpVersion, _device.IpAddress, _port,
                sensor.SnmpSensor.CommunityString, sensor.SnmpSensor.Oid, _login.AuthenticationPassword, _login.PrivacyPassword, _login.SecurityName);
            foreach (var item in results)
            {
                _database.InsertNewSnmpRecord(Guid.NewGuid(), item.Data.ToString(), DateTime.Now.Ticks, sensor.Id);
            }
        }

        return Task.CompletedTask;
    }

    private void InitializeJob(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        _id = (string)dataMap["id"];
        _device = (PhysicalDevice)dataMap["physicalDevice"];
        _sensors = (List<SnmpSensorInPhysicalDevice>)dataMap["sensors"];
        _port = (int)dataMap["port"];
        _login = (LoginProfile)dataMap["loginProfile"];

        Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Job started...");
        Console.Out.WriteLine($"Device name: {_device.Name}");
        Console.Out.WriteLine($"Sensors count: {_sensors.Count()}");
    }
}