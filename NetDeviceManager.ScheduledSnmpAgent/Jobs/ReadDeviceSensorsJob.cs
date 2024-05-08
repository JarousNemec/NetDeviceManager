using System.Text.Json;
using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Snmp.Interfaces;
using NetDeviceManager.Lib.Snmp.Utils;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;

namespace NetDeviceManager.ScheduledSnmpAgent.Jobs;

public class ReadDeviceSensorsJob : IJob
{
    private List<SnmpSensor> _sensors;
    private Port _port;
    private LoginProfile _login;
    private PhysicalDevice _device;
    private string? _id;
    private readonly IDatabaseService _databaseService;
    private readonly ISnmpService _snmpService;

    public ReadDeviceSensorsJob(IDatabaseService databaseService, ISnmpService snmpService)
    {
        _databaseService = databaseService;
        _snmpService = snmpService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        InitializeJob(context);

        foreach (var sensor in _sensors)
        {
            var results = _snmpService.GetSensorValue(sensor, _login, _device, _port);
            var time = DateTime.Now;
            var record = new SnmpSensorRecord();
            var data = new string[sensor.EndIndex-sensor.StartIndex+1];
            foreach (var result in results)
            {
                data[result.Index] = result.Value;
            }
            record.Data = JsonSerializer.Serialize(data);
            record.CapturedTime = time;
            record.PhysicalDeviceId = _device.Id;
            record.SensorId = sensor.Id;
            _databaseService.AddSnmpRecord(record);
        }

        return Task.CompletedTask;
    }

    private void InitializeJob(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        _id = (string)dataMap["id"];
        _device = (PhysicalDevice)dataMap["physicalDevice"];
        _sensors = (List<SnmpSensor>)dataMap["sensors"];
        _port = (Port)dataMap["port"];
        _login = (LoginProfile)dataMap["loginProfile"];

        Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Job started...");
        Console.Out.WriteLine($"Device name: {_device.Name}");
        Console.Out.WriteLine($"Sensors count: {_sensors.Count()}");
    }
}