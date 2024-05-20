using System.Text.Json;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;

namespace NetDeviceManager.Lib.Helpers;

public static class SnmpServiceHelper
{
    public static void CalculateSnmpAlerts(IDatabaseService database, List<Guid> snmpProblemDevice)
    {
        snmpProblemDevice.Clear();
        var patterns = database.GetPhysicalDevicesPatterns();
        foreach (var pattern in patterns)
        {
            var lastRecord = database.GetLastDeviceRecord(pattern.PhysicalDeviceId);
            if(lastRecord == null)
                continue;
            string[] patternData = JsonSerializer.Deserialize<string[]>(pattern.Data);
            if (patternData == null)
                continue;
        
        
            if (pattern.HasToleration)
            {
                string[] data = JsonSerializer.Deserialize<string[]>(lastRecord.Data);
                if (data == null)
                {
                    snmpProblemDevice.Add(pattern.PhysicalDeviceId);
                    continue;
                }
        
                if (int.TryParse(data[0], out int recordValue) && int.TryParse(patternData[0], out int paternValue))
                {
                    if (!(recordValue == paternValue || recordValue > paternValue - pattern.Toleration || recordValue < paternValue + pattern.Toleration))
                    {
                        snmpProblemDevice.Add(pattern.PhysicalDeviceId);
                        continue;
                    }
                }
            }
            else
            {
                if (pattern.Data != lastRecord.Data)
                {
                    snmpProblemDevice.Add(pattern.PhysicalDeviceId);
                    continue;
                }
            }
        }
    }
}