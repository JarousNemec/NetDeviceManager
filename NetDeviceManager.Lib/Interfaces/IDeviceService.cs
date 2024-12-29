using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IDeviceService
{
    OperationResult UpdateIpAddressesAndDeviceRelations(List<string> ipAddresses, Guid deviceId);
    OperationResult UpsertPhysicalDevice(PhysicalDevice model, out Guid id);
    //Read
    int GetOnlineDevicesCount();
    List<PhysicalDevice> GetOnlineDevices();

    int GetOfflineDevicesCount();

    Guid AddPhysicalDevice(PhysicalDevice physicalDevice);
    Guid? AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice);
    
    Guid AddPhysicalDeviceHasIpAddress(PhysicalDeviceHasIpAddress physicalDeviceHasIpAddress);
    void UpdatePhysicalDevice(PhysicalDevice model);
    
    SchedulerJob? GetPhysicalDeviceSchedulerJob(Guid id);
    List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId);

    PhysicalDevice? GetPhysicalDeviceByIp(string ip);

    List<PhysicalDevice> GetAllPhysicalDevices();
    
    List<PhysicalDevice> GetPhysicalDevicesWithIpAddresses();
    List<PhysicalDeviceHasIpAddress> GetPhysicalDeviceIpAddressesRelations(Guid deviceId);

    List<CorrectDataPattern> GetPhysicalDevicesPatterns();
    
    List<string> GetPhysicalDeviceIpAddresses(Guid deviceId);
    OperationResult DeletePhysicalDevice(Guid id);
    OperationResult DeleteSnmpSensorInPhysicalDevice(Guid id);
    OperationResult DeleteSyslogsOfDevice(Guid? physicalDeviceId);

    OperationResult DeletePhysicalDeviceHasIpAddress(Guid id);
    
    int GetPhysicalDeviceSensorsCount(Guid id);
}