using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class LoginProfileServiceFacade(LoginProfileService loginProfileService, ILogger<LoginProfileService> logger) : ILoginProfileService
{
    public OperationResult UpdateLoginProfilesAndDeviceRelations(List<LoginProfile> profiles, Guid deviceId)
    {
        var result = loginProfileService.UpdateLoginProfilesAndDeviceRelations(profiles, deviceId);
        logger.LogInformation($"Updated relations between device with id: {deviceId} and login profiles");
        return result;
    }

    public Guid AddLoginProfile(LoginProfile profile)
    {
        var result = loginProfileService.AddLoginProfile(profile);
        logger.LogInformation($"Added login profile: {result}");
        return result;
    }

    public Guid UpsertLoginProfile(LoginProfile profile)
    {
        var result = loginProfileService.UpsertLoginProfile(profile);
        logger.LogInformation($"Upserted login profile: {result}");
        return result;
    }

    public List<LoginProfile> GetAllLoginProfiles()
    {
        var result = loginProfileService.GetAllLoginProfiles();
        logger.LogInformation($"Found {result.Count} login profiles in system");
        return result;
    }

    public LoginProfile? GetLoginProfile(Guid id)
    {
        var result = loginProfileService.GetLoginProfile(id);
        logger.LogInformation((result != null
            ? $"Found login profile with id: {id}"
            : $"Can't find login profile with id: {id}"));
        return result;
    }

    public List<LoginProfile> GetPhysicalDeviceLoginProfiles(Guid deviceId)
    {
        var result = loginProfileService.GetPhysicalDeviceLoginProfiles(deviceId);
        logger.LogInformation($"Found {result.Count} login profiles for device: {deviceId}");
        return result;
    }

    public List<LoginProfileToPhysicalDevice> GetPhysicalDeviceLoginProfileRelationships(Guid deviceId)
    {
        var result = loginProfileService.GetPhysicalDeviceLoginProfileRelationships(deviceId);
        logger.LogInformation($"Found {result.Count} login profiles relations for device with id: {deviceId}");
        return result;
    }

    public Guid AssignLoginProfileToPhysicalDevice(LoginProfileToPhysicalDevice profile)
    {
        var result = loginProfileService.AssignLoginProfileToPhysicalDevice(profile);
        logger.LogInformation($"Assigned login profile to device with relation id: {result}");
        return result;
    }

    public OperationResult RemoveLoginProfileFromPhysicalDevice(Guid relationId)
    {
        var result = loginProfileService.RemoveLoginProfileFromPhysicalDevice(relationId);
        logger.LogInformation(result.IsSuccessful
            ? $"Removed login profile from device with relation id: {relationId}"
            : $"Can't remove login profile from device with relation id: {relationId} with message: {result.Message}");
        return result;
    }

    public OperationResult RemoveLoginProfile(Guid id)
    {
        var result = loginProfileService.RemoveLoginProfile(id);
        logger.LogInformation(result.IsSuccessful
            ? $"Removed login profile with id: {id}"
            : $"Can't remove login profile with id: {id} with message: {result.Message}");
        return result;
    }
}