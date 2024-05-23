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
    List<PhysicalDevice> GetOfflineDevices();

    List<Device> GetDevices();
    List<PhysicalDevice> GetPhysicalDevices();
    List<DeviceIcon> GetDevicesIcons();
    List<LoginProfile> GetLoginProfiles();

    Device GetDevice(Guid id);
    PhysicalDevice GetPhysicalDevice(Guid id);
    DeviceIcon GetDeviceIcon(Guid id);
    LoginProfile GetLoginProfile(Guid id);

    Port GetPort(Guid id);

    //Update
    OperationResult UpdateDevice(Guid id, CreateDeviceModel model);
    OperationResult UpdatePhysicalDevice(Guid id, PhysicalDevice model);
    OperationResult UpdateDeviceIcon(Guid id, CreateDeviceIconModel model);
    OperationResult UpdateLoginProfile(Guid id, CreateLoginProfileModel model);

    OperationResult UpdatePort(Port model);

    //Delete
    OperationResult DeleteDevice(Guid id);
    OperationResult DeletePhysicalDevice(Guid id);
    OperationResult DeleteDeviceIcon(Guid id);
    OperationResult DeleteLoginProfile(Guid id);

    OperationResult DeletePort(Guid id);
}