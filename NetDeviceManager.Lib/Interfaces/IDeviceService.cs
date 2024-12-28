using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IDeviceService
{
    //Create
    OperationResult AddDevice(CreateDeviceModel model);
    OperationResult UpsertPhysicalDevice(PhysicalDevice model, out Guid id);
    Task<DeviceIcon> AddDeviceIcon(CreateDeviceIconModel model);
    OperationResult AddLoginProfile(CreateLoginProfileModel model);

    OperationResult AddPortToDevice(Port model, Guid deviceId);
    //Read
    int GetOnlineDevicesCount();
    List<PhysicalDevice> GetOnlineDevices();

    int GetOfflineDevicesCount();
}