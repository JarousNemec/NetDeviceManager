using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface ILoginProfileService
{
    
    Guid AddLoginProfile(LoginProfile profile);

    Guid UpsertLoginProfile(LoginProfile profile);

    List<LoginProfile> GetAllLoginProfiles();
    LoginProfile? GetLoginProfile(Guid id);

    List<LoginProfile> GetPhysicalDeviceLoginProfiles(Guid deviceId);
    List<LoginProfileToPhysicalDevice> GetPhysicalDeviceLoginProfileRelationships(Guid deviceId);

    Guid AssignLoginProfileToPhysicalDevice(LoginProfileToPhysicalDevice profile);

    OperationResult RemoveLoginProfileFromPhysicalDevice(Guid relationId);
    
    OperationResult UpdateLoginProfilesAndDeviceRelations(List<LoginProfile> profiles, Guid deviceId);

    OperationResult RemoveLoginProfile(Guid id);
}