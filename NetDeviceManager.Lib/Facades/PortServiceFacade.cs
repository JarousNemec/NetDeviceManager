using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class PortServiceFacade(PortService portService, ILogger<PortService> logger) : IPortService
{
    public OperationResult UpdatePortsAndDeviceRelations(List<Port> ports, Guid deviceId)
    {
        var result = portService.UpdatePortsAndDeviceRelations(ports, deviceId);
        logger.LogInformation($"Ports updated");
        return result;
    }

    public Guid UpsertPort(Port port)
    {
        var result = portService.UpsertPort(port);
        logger.LogInformation($"Upserted Port with id: {port.Id}");
        return result;
    }

    public Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort)
    {
        var result = portService.AddPortToPhysicalDevice(physicalDeviceHasPort);
        logger.LogInformation(
            $"Added relation between device with id: {physicalDeviceHasPort.DeviceId} and port with id: {physicalDeviceHasPort.PortId}");
        return result;
    }

    public Guid? GetPortDeviceRelationId(Guid portId, Guid deviceId)
    {
        var result = portService.GetPortDeviceRelationId(portId, deviceId);
        logger.LogInformation($"Got port device relation Id: {result} for port Id: {portId} and device Id: {deviceId}");
        return result;
    }

    public List<Port> GetPortsInPhysicalDevice(Guid deviceId)
    {
        var result = portService.GetPortsInPhysicalDevice(deviceId);
        logger.LogInformation(
            $"Got ports with ids: {string.Join(", ", result.Select(x => x.Number.ToString()))} for device with id: {deviceId}");
        return result;
    }

    public List<PhysicalDeviceHasPort> GetPortInPhysicalDeviceRelations(Guid deviceId)
    {
        var result = portService.GetPortInPhysicalDeviceRelations(deviceId);
        logger.LogInformation($"Got {result.Count} port device relations for device with id: {deviceId}");
        return result;
    }

    public List<Port> GetPortsInSystem()
    {
        var result = portService.GetPortsInSystem();
        logger.LogInformation($"Got {result.Count} port in system");
        return result;
    }

    public OperationResult RemovePortFromDevice(Guid id)
    {
        var result = portService.RemovePortFromDevice(id);
        logger.LogInformation($"{(result.IsSuccessful ? "Removed" : "Failed to remove")} port device relation with id: {id}");
        return result;
    }

    public OperationResult RemovePort(Guid id)
    {
        var result = portService.RemovePort(id);
        logger.LogInformation($"{(result.IsSuccessful ? "Removed" : "Failed to remove")} port with id: {id}");
        return result;
    }

    public bool PortAndDeviceRelationExists(Guid portId, Guid deviceId)
    {
        var result = portService.PortAndDeviceRelationExists(portId, deviceId);
        logger.LogInformation($"Relation between device with id: {deviceId} and port with id: {portId} {(result?"EXISTS":"NOT EXISTS")}");
        return result;
    }
}