using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;

namespace NetDeviceManager.Lib.Helpers;

public static class DeviceServiceHelper
{
    // public static void CalculateOnlineOfflineDevices(IDeviceService deviceService, List<PhysicalDevice> online, List<PhysicalDevice> offline)
    // {
    //     var devices = deviceService.GetAllPhysicalDevices();
    //     online.Clear();
    //     offline.Clear();
    //     var maxAge = TimeSpan.TicksPerMinute * 15;
    //     foreach (var device in devices)
    //     {
    //         var lastRecord = database.GetLastDeviceRecord(device.Id);
    //         if (lastRecord == null)
    //         {
    //             offline.Add(device);
    //             continue;
    //         }
    //
    //         if ((DateTime.Now - lastRecord.CapturedTime).Ticks > maxAge)
    //         {
    //             offline.Add(device);
    //         }
    //         else
    //         {
    //             online.Add(device);
    //         }
    //     }
    // }
}