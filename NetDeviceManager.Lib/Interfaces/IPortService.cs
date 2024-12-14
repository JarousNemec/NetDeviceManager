using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IPortService
{
    OperationResult UpdatePortsAndDeviceRelations(List<Port> ports, Guid deviceId);
}