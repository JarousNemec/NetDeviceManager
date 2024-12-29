using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class DeviceServiceFacade(DeviceService deviceService, ILogger<DeviceService> logger) : IDeviceService
{
    public OperationResult UpdateIpAddressesAndDeviceRelations(List<string> ipAddresses, Guid deviceId)
    {
        var result = new OperationResult();
        logger.LogInformation($"Updated list of IP addresses assigned to device with id: {deviceId}");
        return result;
    }

    public OperationResult UpsertPhysicalDevice(PhysicalDevice model, out Guid id)
    {
        var result = deviceService.UpsertPhysicalDevice(model, out id);
        logger.LogInformation($"{(result.IsSuccessful ? "Upserted" : "Cannot upsert")} device id: {id}");
        return result;
    }

    public int GetOnlineDevicesCount()
    {
        var result = deviceService.GetOnlineDevicesCount();
        logger.LogInformation($"Retrieved count {result} of online devices");
        return result;
    }

    public List<PhysicalDevice> GetOnlineDevices()
    {
        var result = deviceService.GetOnlineDevices();
        logger.LogInformation($"Retrieved {result.Count} online devices");
        return result;
    }

    public int GetOfflineDevicesCount()
    {
        var result = deviceService.GetOfflineDevicesCount();
        logger.LogInformation($"Retrieved count {result} of offline devices");
        return result;
    }

    public Guid AddPhysicalDevice(PhysicalDevice physicalDevice)
    {
        var result = deviceService.AddPhysicalDevice(physicalDevice);
        logger.LogInformation($"Added physical device with id: {result}");
        return result;
    }

    public Guid? AddSnmpSensorToPhysicalDevice(SnmpSensorInPhysicalDevice sensorInPhysicalDevice)
    {
        var result = deviceService.AddSnmpSensorToPhysicalDevice(sensorInPhysicalDevice);
        logger.LogInformation(
            $"Added snmp sensor device relation with id: {result} for sensor id: {sensorInPhysicalDevice.SnmpSensorId} and device id: {sensorInPhysicalDevice.PhysicalDeviceId}");
        return result;
    }

    public Guid AddPhysicalDeviceHasIpAddress(PhysicalDeviceHasIpAddress physicalDeviceHasIpAddress)
    {
        var result = deviceService.AddPhysicalDeviceHasIpAddress(physicalDeviceHasIpAddress);
        logger.LogInformation(
            $"Added relation with id: {result} between physical device and ip address: {physicalDeviceHasIpAddress.IpAddress}");
        return result;
    }

    public void UpdatePhysicalDevice(PhysicalDevice model)
    {
        deviceService.UpdatePhysicalDevice(model);
        logger.LogInformation($"Updated physical device with id: {model.Id}");
    }

    public SchedulerJob? GetPhysicalDeviceSchedulerJob(Guid id)
    {
        var result = deviceService.GetPhysicalDeviceSchedulerJob(id);
        logger.LogInformation($"{(result == null ? "Retrieved" : "Cannot retrieve")} scheduler job with id: {id}");
        return result;
    }

    public List<SnmpSensorInPhysicalDevice> GetSensorsOfPhysicalDevice(Guid physicalDeviceId)
    {
        var result = deviceService.GetSensorsOfPhysicalDevice(physicalDeviceId);
        logger.LogInformation($"Retrieved {result.Count} sensors for device with id: {physicalDeviceId}");
        return result;
    }

    public PhysicalDevice? GetPhysicalDeviceByIp(string ip)
    {
        var result = deviceService.GetPhysicalDeviceByIp(ip);
        logger.LogInformation(
            $"{(result == null ? "Cannot retrieve device" : $"Retrived device with id: {result.Id}")} by ip: {ip}");
        return result;
    }

    public List<PhysicalDevice> GetAllPhysicalDevices()
    {
        var result = deviceService.GetAllPhysicalDevices();
        logger.LogInformation($"Retrieved {result.Count} physical devices from system");
        return result;
    }

    public List<PhysicalDevice> GetPhysicalDevicesWithIpAddresses()
    {
        var result = deviceService.GetPhysicalDevicesWithIpAddresses();
        logger.LogInformation($"Retrieved {result.Count} physical devices with included ip addresses from system");
        return result;
    }

    public List<PhysicalDeviceHasIpAddress> GetPhysicalDeviceIpAddressesRelations(Guid deviceId)
    {
        var result = deviceService.GetPhysicalDeviceIpAddressesRelations(deviceId);
        logger.LogInformation($"Retrieved {result.Count} physical devices relation with ip addresses from system");
        return result;
    }

    public List<CorrectDataPattern> GetPhysicalDevicesPatterns()
    {
        var result = deviceService.GetPhysicalDevicesPatterns();
        logger.LogInformation($"Retrieved {result.Count} physical devices patterns");
        return result;
    }

    public List<string> GetPhysicalDeviceIpAddresses(Guid deviceId)
    {
        var result = deviceService.GetPhysicalDeviceIpAddresses(deviceId);
        logger.LogInformation($"Retrieved {result.Count} ip addresses for device with id: {deviceId}");
        return result;
    }

    public OperationResult DeletePhysicalDevice(Guid id)
    {
        var result = deviceService.DeletePhysicalDevice(id);
        logger.LogInformation(
            $"{(result.IsSuccessful ? "Deleted" : $"Error: {result.Message} so cannot delete")} PhysicalDevice with id: {id}");
        return result;
    }

    public OperationResult DeleteSnmpSensorInPhysicalDevice(Guid id)
    {
        var result = deviceService.DeleteSnmpSensorInPhysicalDevice(id);
        logger.LogInformation(
            $"{(result.IsSuccessful ? $"Removed" : $"Error: {result.Message} so cannot remove")} snmp sensor and device relation with id: {id}");
        return result;
    }

    public OperationResult DeleteSyslogsOfDevice(Guid? physicalDeviceId)
    {
        var result = deviceService.DeleteSyslogsOfDevice(physicalDeviceId);
        logger.LogInformation(
            $"{(result.IsSuccessful ? "Successfully removed" : $"Error: {result.Message} so cannot remove")} syslogs of device with id: {physicalDeviceId}");
        return result;
    }

    public OperationResult DeletePhysicalDeviceHasIpAddress(Guid id)
    {
        var result = deviceService.DeletePhysicalDeviceHasIpAddress(id);
        logger.LogInformation(
            $"{(result.IsSuccessful ? "Successfully removed" : $"Error: {result.Message} so cannot remove")} ip address and device relation with id: {id}");
        return result;
    }

    public int GetPhysicalDeviceSensorsCount(Guid id)
    {
        var result = deviceService.GetPhysicalDeviceSensorsCount(id);
        logger.LogInformation($"Retrieved sum {result} of sensors for device with id: {id}");
        return result;
    }
}