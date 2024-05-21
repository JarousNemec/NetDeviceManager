using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IDeviceService
{
    //Create
    public OperationResult AddDevice(CreateDeviceModel model);
    public OperationResult AddPhysicalDevice(PhysicalDevice model);
    public OperationResult AddDeviceIcon(CreateDeviceIconModel model);
    public OperationResult AddLoginProfile(CreateLoginProfileModel model);

    public OperationResult AddPort(Port model);
    
    //Read
    public int GetOnlineDevicesCount();
    public List<PhysicalDevice> GetOnlineDevices();
    
    public int GetOfflineDevicesCount();
    public List<PhysicalDevice> GetOfflineDevices();
    
    public List<Device> GetDevices();
    public List<PhysicalDevice> GetPhysicalDevices();
    public List<DeviceIcon> GetDevicesIcons();
    public List<LoginProfile> GetLoginProfiles();
    
    public Device GetDevice(Guid id);
    public PhysicalDevice GetPhysicalDevice(Guid id);
    public DeviceIcon GetDeviceIcon(Guid id);
    public LoginProfile GetLoginProfile(Guid id);

    public Port GetPort(Guid id);
    
    //Update
    public OperationResult UpdateDevice(Guid id, CreateDeviceModel model);
    public OperationResult UpdatePhysicalDevice(Guid id, PhysicalDevice model);
    public OperationResult UpdateDeviceIcon(Guid id, CreateDeviceIconModel model);
    public OperationResult UpdateLoginProfile(Guid id, CreateLoginProfileModel model);

    public OperationResult UpdatePort(Port model);
    
    //Delete
    public OperationResult DeleteDevice(Guid id);
    public OperationResult DeletePhysicalDevice(Guid id);
    public OperationResult DeleteDeviceIcon(Guid id);
    public OperationResult DeleteLoginProfile(Guid id);

    public OperationResult DeletePort(Guid id);
}