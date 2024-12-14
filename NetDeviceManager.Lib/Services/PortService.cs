using System.Diagnostics;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class PortService : IPortService
{
    private readonly IDatabaseService _databaseService;

    public PortService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    public OperationResult UpdatePortsAndDeviceRelations(List<Port> ports, Guid deviceId)
    {
        var currentRelations = _databaseService.GetPortInPhysicalDeviceRelations(deviceId);
        var toAdd = new List<PhysicalDeviceHasPort>();
        var toRemove = new List<PhysicalDeviceHasPort>();
        foreach (var port in ports)
        {
            if (currentRelations.All(x => x.Port.Number != port.Number))
            {
                toAdd.Add(new PhysicalDeviceHasPort(){PortId = port.Id, DeviceId = deviceId});
            }
        }

        foreach (var relation in currentRelations)
        {
            if (ports.All(x => x.Number != relation.Port.Number))
            {
                toRemove.Add(relation);
            }
        }

        try
        {
            foreach (var relationToAdd in toAdd)
            {
                _databaseService.AddPortToPhysicalDevice(relationToAdd);
            }

            foreach (var relationToRemove in toRemove)
            {
                _databaseService.RemovePortFromDevice(relationToRemove.Id);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return new OperationResult(){IsSuccessful = false, Message = e.Message };
        }
        
        
        return new OperationResult();
    }
}