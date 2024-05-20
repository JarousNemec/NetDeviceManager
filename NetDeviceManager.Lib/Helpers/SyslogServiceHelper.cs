using NetDeviceManager.Lib.Interfaces;

namespace NetDeviceManager.Lib.Helpers;

public static class SyslogServiceHelper
{
    public static void CalculateAlertSyslogs(IDatabaseService database, List<Guid> logs)
    {
        logs.Clear();
        logs.AddRange(database.GetSyslogs());
    }
}