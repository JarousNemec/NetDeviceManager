using System.Diagnostics;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class LoginProfileService : ILoginProfileService
{
    private readonly IDatabaseService _databaseService;

    public LoginProfileService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    public OperationResult UpdateLoginProfilesAndDeviceRelations(List<LoginProfile> profiles, Guid deviceId)
    {
        var currentRelations = _databaseService.GetPhysicalDeviceLoginProfileRelationships(deviceId);
        var toAdd = new List<LoginProfileToPhysicalDevice>();
        var toRemove = new List<LoginProfileToPhysicalDevice>();
        
        foreach (var profile in profiles)
        {
            if (currentRelations.All(x => x.LoginProfileId != profile.Id && x.PhysicalDeviceId != deviceId))
            {
                toAdd.Add(new LoginProfileToPhysicalDevice(){LoginProfileId = profile.Id, PhysicalDeviceId = deviceId});
            }
        }

        foreach (var profile in currentRelations)
        {
            if (profiles.All(x => x.Id != profile.Id))
            {
                toRemove.Add(profile);
            }
        }

        try
        {
            foreach (var relationToAdd in toAdd)
            {
                _databaseService.AssignLoginProfileToPhysicalDevice(relationToAdd);
            }

            foreach (var relationToRemove in toRemove)
            {
                _databaseService.RemoveLoginProfileFromPhysicalDevice(relationToRemove.Id);
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