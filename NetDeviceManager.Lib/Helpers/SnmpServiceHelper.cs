using System.Text.Json;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Helpers;

public static class SnmpServiceHelper
{
    public static void CalculateSnmpAlerts(IDatabaseService database, List<SnmpAlertModel> snmpProblemDevice)
    {
        snmpProblemDevice.Clear();
        var patterns = database.GetPhysicalDevicesPatterns();
        foreach (var pattern in patterns)
        {
            var lastRecord = database.GetLastDeviceRecord(pattern.PhysicalDeviceId);
            if (lastRecord == null)
                continue;
            string[] patternData = JsonSerializer.Deserialize<string[]>(pattern.Data);
            if (patternData == null)
                continue;


            if (pattern.HasToleration)
            {
                string[] data = JsonSerializer.Deserialize<string[]>(lastRecord.Data);

                //if no data in last record
                if (data == null)
                {
                    snmpProblemDevice.Add(new SnmpAlertModel()
                    {
                        Id = Guid.NewGuid(),
                        Device = pattern.PhysicalDevice,
                        Sensor = pattern.Sensor,
                        Current = lastRecord.Data,
                        Expected = pattern.Data
                    });
                    continue;
                }

                //if assertion has toleration in number value
                if (int.TryParse(data[0], out int recordValue) && int.TryParse(patternData[0], out int paternValue))
                {
                    if (!(recordValue == paternValue || recordValue > paternValue - pattern.Toleration ||
                          recordValue < paternValue + pattern.Toleration))
                    {
                        snmpProblemDevice.Add(new SnmpAlertModel()
                        {
                            Id = Guid.NewGuid(),
                            Device = pattern.PhysicalDevice,
                            Sensor = pattern.Sensor,
                            Current = lastRecord.Data,
                            Expected = pattern.Data
                        });
                        continue;
                    }
                }
            }
            else
            {
                //if hasnt toleration and data arent equal
                if (pattern.Data != lastRecord.Data)
                {
                    snmpProblemDevice.Add(new SnmpAlertModel()
                    {
                        Id = Guid.NewGuid(),
                        Device = pattern.PhysicalDevice,
                        Sensor = pattern.Sensor,
                        Current = lastRecord.Data,
                        Expected = pattern.Data
                    });
                    continue;
                }
            }
        }
    }
}