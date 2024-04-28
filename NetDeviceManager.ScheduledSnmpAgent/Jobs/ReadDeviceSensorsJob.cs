using FluentScheduler;
using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.ScheduledSnmpAgent.Jobs;

public class ReadDeviceSensorsJob : IJob
{
    private readonly List<SnmpSensor> _sensors;
    private readonly PhysicalDevice _device;
    private readonly string _id;
    public ReadDeviceSensorsJob(string id, List<SnmpSensor> sensors, PhysicalDevice device)
    {
        _sensors = sensors;
        _device = device;
        _id = id;
    }
    public void Execute()
    {
        Console.WriteLine($"{_id} - ({DateTime.Now}) - Job started....");
    }

    public string GetId()
    {
        return _id;
    }
}