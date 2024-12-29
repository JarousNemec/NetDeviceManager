using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IPortService
{
    
    Guid UpsertPort(Port port);

    Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort);
    OperationResult UpdatePortsAndDeviceRelations(List<Port> ports, Guid deviceId);
    Guid? GetPortDeviceRelationId(Guid portId, Guid deviceId);
    List<Port> GetPortsInPhysicalDevice(Guid deviceId);
    List<PhysicalDeviceHasPort> GetPortInPhysicalDeviceRelations(Guid deviceId);
    OperationResult RemovePortFromDevice(Guid id);
    bool PortAndDeviceRelationExists(Guid portId, Guid deviceId);
    List<Port> GetPortsInSystem();

    OperationResult RemovePort(Guid id);

    
}