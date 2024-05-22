using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class DeviceService : IDeviceService
{
    private readonly IDatabaseService _database;
    private readonly IFileStorageService _fileStorageService;

    public DeviceService(IDatabaseService database, IFileStorageService fileStorageService)
    {
        _database = database;
        _fileStorageService = fileStorageService;
    }

    #region CreateMethods

    public OperationResult AddDevice(CreateDeviceModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpsertPhysicalDevice(PhysicalDevice model, out Guid id)
    {
        if (model.Id != new Guid())
        {
            _database.UpdatePhysicalDevice(model);
            id = model.Id;
            return new OperationResult();
        }

        if (!_database.AnyPhysicalDeviceWithIp(model.IpAddress))
        {
            id = _database.AddPhysicalDevice(model);
            return new OperationResult();
        }

        id = default;
        return new OperationResult() { IsSuccessful = false, Message = "Device ip is already assigned!" };
    }

    public DeviceIcon AddDeviceIcon(CreateDeviceIconModel model)
    {
        var icon = new DeviceIcon();
        icon.Name = model.Name;
        icon.Description = model.Description;
        var id = _database.AddDeviceIcon(icon);
        _fileStorageService.SaveIconFile(id, model.File);
        icon.Id = id;
        return icon;
    }

    public OperationResult AddLoginProfile(CreateLoginProfileModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult AddPortToDevice(Port model, Guid deviceId)
    {
        Guid portId;
        if (!_database.PortExists(model, out portId))
        {
            portId = _database.AddPort(model);
        }

        if (!_database.PortAddDeviceRelationExists(portId, deviceId, out _))
        {
            var relationship = new PhysicalDeviceHasPort()
            {
                DeviceId = deviceId,
                PortId = portId
            };
            _database.AddPortToPhysicalDevice(relationship);
        }

        return new OperationResult();
    }
    #endregion

    #region GetMethods

    private readonly List<PhysicalDevice> _onlineDevices = new List<PhysicalDevice>();
    private readonly List<PhysicalDevice> _offlineDevices = new List<PhysicalDevice>();
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);

    public int GetOnlineDevicesCount()
    {
        CheckTimelinessOfData();
        return _onlineDevices.Count;
    }

    public List<PhysicalDevice> GetOnlineDevices()
    {
        CheckTimelinessOfData();
        return _onlineDevices;
    }

    public int GetOfflineDevicesCount()
    {
        CheckTimelinessOfData();
        return _offlineDevices.Count;
    }

    public List<PhysicalDevice> GetOfflineDevices()
    {
        CheckTimelinessOfData();
        return _offlineDevices;
    }

    private void CheckTimelinessOfData()
    {
        if ((DateTime.Now.Ticks - _lastUpdate.Ticks) > (TimeSpan.TicksPerMinute * 5))
        {
            DeviceServiceHelper.CalculateOnlineOfflineDevices(_database, _onlineDevices, _offlineDevices);
            _lastUpdate = DateTime.Now;
        }
    }

    public List<Device> GetDevices()
    {
        throw new NotImplementedException();
    }

    public List<PhysicalDevice> GetPhysicalDevices()
    {
        throw new NotImplementedException();
    }

    public List<DeviceIcon> GetDevicesIcons()
    {
        throw new NotImplementedException();
    }

    public List<LoginProfile> GetLoginProfiles()
    {
        throw new NotImplementedException();
    }

    public Device GetDevice(Guid id)
    {
        throw new NotImplementedException();
    }

    public PhysicalDevice GetPhysicalDevice(Guid id)
    {
        throw new NotImplementedException();
    }

    public DeviceIcon GetDeviceIcon(Guid id)
    {
        throw new NotImplementedException();
    }

    public LoginProfile GetLoginProfile(Guid id)
    {
        throw new NotImplementedException();
    }

    public Port GetPort(Guid id)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region UpdateMethods

    public OperationResult UpdateDevice(Guid id, CreateDeviceModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpdatePhysicalDevice(Guid id, PhysicalDevice model)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpdateDeviceIcon(Guid id, CreateDeviceIconModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpdateLoginProfile(Guid id, CreateLoginProfileModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpdatePort(Port model)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region DeleteMethods

    public OperationResult DeleteDevice(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationResult DeletePhysicalDevice(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationResult DeleteDeviceIcon(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationResult DeleteLoginProfile(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationResult DeletePort(Guid id)
    {
        throw new NotImplementedException();
    }

    #endregion
}