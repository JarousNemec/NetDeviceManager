using System.Text.Json;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;

namespace NetDeviceManager.ScheduledSnmpAgent.Jobs;

public class ReadDeviceSensorsJob : IJob
{
    private List<SnmpSensor> _sensors;
    private Port _port;
    private List<LoginProfile> _logins;
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
            var data = _snmpService.GetSensorValue(sensor, _logins, _device, _port) ?? string.Empty;
            var record = new SnmpSensorRecord
            {
                Data = data,
                CapturedTime = DateTime.Now,
                PhysicalDeviceId = _device.Id,
                SensorId = sensor.Id
            };
            _databaseService.AddSnmpRecord(record);
        }

        return Task.CompletedTask;
    }

    private void InitializeJob(IJobExecutionContext context)
    {
        //todo presun nazvu do konstant
        var dataMap = context.MergedJobDataMap;

        _id = (string)dataMap["id"];
        _device = (PhysicalDevice)dataMap["physicalDevice"];
        _sensors = (List<SnmpSensor>)dataMap["sensors"];
        _port = (Port)dataMap["port"];
        _logins = (List<LoginProfile>)dataMap["loginProfiles"];

        Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Job started...");
        Console.Out.WriteLine($"Device name: {_device.Name}");
        Console.Out.WriteLine($"Sensors count: {_sensors.Count()}");
    }
}