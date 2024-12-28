using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IPortService
{
    OperationResult UpdatePortsAndDeviceRelations(List<Port> ports, Guid deviceId);
    Guid UpsertPort(Port port);

    Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort);

    Guid? GetPortDeviceRelationId(Guid portId, Guid deviceId);
    List<Port> GetPortsInPhysicalDevice(Guid deviceId);
    List<PhysicalDeviceHasPort> GetPortInPhysicalDeviceRelations(Guid deviceId);
    List<Port> GetPortsInSystem();
    OperationResult RemovePortFromDevice(Guid id);

    OperationResult RemovePort(Guid id);

    bool PortAndDeviceRelationExists(Guid portId, Guid deviceId);
}