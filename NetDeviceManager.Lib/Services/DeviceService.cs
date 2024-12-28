using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class DeviceService : IDeviceService
{
    private readonly IDatabaseService _database;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPortService _portService;

    private readonly List<PhysicalDevice> _onlineDevices = new List<PhysicalDevice>();
    private readonly List<PhysicalDevice> _offlineDevices = new List<PhysicalDevice>();
    private DateTime _lastUpdate = new DateTime(2006, 8, 1, 20, 20, 20);

    public DeviceService(IDatabaseService database, IFileStorageService fileStorageService, IPortService portService)
    {
        _database = database;
        _fileStorageService = fileStorageService;
        _portService = portService;
    }

    public OperationResult AddDevice(CreateDeviceModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult UpsertPhysicalDevice(PhysicalDevice model, out Guid id)
    {
        if (model.Id != default)
        {
            _database.UpdatePhysicalDevice(model);
            id = model.Id;
            return new OperationResult();
        }
        id = _database.AddPhysicalDevice(model);
        return new OperationResult();

        id = default;
        return new OperationResult() { IsSuccessful = false, Message = "Device ip is already assigned!" };
    }

    public async Task<DeviceIcon> AddDeviceIcon(CreateDeviceIconModel model)
    {
        var icon = new DeviceIcon();
        icon.Name = model.Name;
        icon.Description = model.Description;
        var id = _database.AddDeviceIcon(icon);
        await _fileStorageService.SaveIconFile(id, model.File);
        icon.Id = id;
        return icon;
    }

    public OperationResult AddLoginProfile(CreateLoginProfileModel model)
    {
        throw new NotImplementedException();
    }

    public OperationResult AddPortToDevice(Port model, Guid deviceId)
    {
        var portId = _portService.UpsertPort(model);

        if (!_portService.PortAndDeviceRelationExists(portId, deviceId))
        {
            var relationship = new PhysicalDeviceHasPort()
            {
                DeviceId = deviceId,
                PortId = portId
            };
            _portService.AddPortToPhysicalDevice(relationship);
        }

        return new OperationResult();
    }
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
}