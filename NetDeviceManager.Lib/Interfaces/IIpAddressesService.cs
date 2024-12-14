using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IIpAddressesService
{
    OperationResult UpdateIpAddressesAndDeviceRelations(List<string> ipAddresses, Guid deviceId);
    
}