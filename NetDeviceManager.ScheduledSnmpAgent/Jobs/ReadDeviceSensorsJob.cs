using System.Text.Json;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.ScheduledSnmpAgent.Utils;
using Quartz;

namespace NetDeviceManager.ScheduledSnmpAgent.Jobs;

public class ReadDeviceSensorsJob : IJob
{
    private List<SnmpSensorInPhysicalDevice> _relationShips;
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

        foreach (var relationShip in _relationShips)
        {
            var data = _snmpService.GetSensorValue(relationShip.SnmpSensor, _login, _device, _port) ?? string.Empty;
            var record = new SnmpSensorRecord
            {
                Data = data,
                CapturedTime = DateTime.Now,
                PhysicalDeviceId = _device.Id,
                SensorId = relationShip.SnmpSensorId
            };
            _databaseService.AddSnmpRecord(record);
        }

        return Task.CompletedTask;
    }

    private void InitializeJob(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        _id = (string)dataMap["id"];
        _device = (PhysicalDevice)dataMap["physicalDevice"];
        _relationShips = _databaseService.GetSensorsOfPhysicalDevice(_device.Id);
        _port = (Port)dataMap["port"];
        _login = (LoginProfile)dataMap["loginProfile"];

        Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Job started...");
        Console.Out.WriteLine($"Device name: {_device.Name}");
        Console.Out.WriteLine($"Sensors count: {_relationShips.Count()}");
    }
}