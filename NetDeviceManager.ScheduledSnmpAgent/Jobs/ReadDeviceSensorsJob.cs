using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using Quartz;

namespace NetDeviceManager.ScheduledSnmpAgent.Jobs;

public class ReadDeviceSensorsJob : IJob
{
    private List<SnmpSensorInPhysicalDevice> _sensors;
    private PhysicalDevice _device;
    private string? _id;
    private ApplicationDbContext _database;

    // public ReadDeviceSensorsJob()
    // {
    //     _database = database;
    // }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        _id = (string)dataMap["id"];
        _device = (PhysicalDevice)dataMap["physicalDevice"];
        _sensors = (List<SnmpSensorInPhysicalDevice>)dataMap["sensors"];
        _database = (ApplicationDbContext)dataMap["database"];
         Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Job started....");
         Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Device name: {_device.Name}");
         Console.Out.WriteLine($"{_id} - ({DateTime.Now}) - Sensors count: {_sensors.Count()}");
        var snmpVersion = VersionCode.V2;
        foreach (var sensor in _sensors)
        {
            switch (sensor.SnmpSensor.SnmpVersion)
            {
                case "1":
                    snmpVersion = VersionCode.V1;
                    break;
                case "2":
                    snmpVersion = VersionCode.V2;
                    break;
                case "3":
                    snmpVersion = VersionCode.V3;
                    break;
            }

            var result = Messenger.Get(snmpVersion,
                new IPEndPoint(IPAddress.Parse(sensor.PhysicalDevice.IpAddress), int.Parse(sensor.PhysicalDevice.Port)),
                new OctetString(sensor.SnmpSensor.Community.CommunityStringValue),
                new List<Variable>
                { new Variable(new ObjectIdentifier(sensor.SnmpSensor.Oid)) }, //1.3.6.1.2.1.1.3.0 for uptime
                10000);

            foreach (var item in result)
            {
                Console.WriteLine($"ID: {item.Id} Data: {item.Data.ToString()}");
                var record = new SnmpSensorRecord()
                {
                    Id = Guid.NewGuid(),
                    Value = item.Data.ToString(),
                    CapturedTime = DateTime.Now.Ticks,
                    SensorInPhysicalDeviceId = sensor.Id
                };
                _database.SnmpSensorRecords.Add(record);
                await _database.SaveChangesAsync();
            }
        }
    }
}