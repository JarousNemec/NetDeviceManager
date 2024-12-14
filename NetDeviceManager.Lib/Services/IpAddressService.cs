using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class IpAddressService : IIpAddressesService
{
    private readonly IDatabaseService _databaseService;

    public IpAddressService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    public OperationResult UpdateIpAddressesAndDeviceRelations(List<string> ipAddresses, Guid deviceId)
    {
        var currentRelations = _databaseService.GetPhysicalDeviceIpAddressesRelations(deviceId);
        var toAdd = new List<PhysicalDeviceHasIpAddress>();
        var toRemove = new List<PhysicalDeviceHasIpAddress>();

        foreach (var ipAddress in ipAddresses.Select(v => v.Trim()))
        {
            if (currentRelations.All(x => x.IpAddress != ipAddress))
            {
                if(IPAddress.TryParse(ipAddress, out IPAddress? ip))
                    toAdd.Add(new PhysicalDeviceHasIpAddress()
                    {
                        IpAddress = ip.ToString(),
                        PhysicalDeviceId = deviceId
                    });
            }
        }

        foreach (var relation in currentRelations)
        {
            if (!ipAddresses.Contains(relation.IpAddress))
            {
                toRemove.Add(relation);
            }
        }

        try
        {
            foreach (var relation in toAdd)
            {
                _databaseService.AddPhysicalDeviceHasIpAddress(relation);
            }

            foreach (var relation in toRemove)
            {
                _databaseService.DeletePhysicalDeviceHasIpAddress(relation.Id);
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