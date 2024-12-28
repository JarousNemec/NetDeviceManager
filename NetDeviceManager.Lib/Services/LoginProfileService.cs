using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class LoginProfileService(ApplicationDbContext database) : ILoginProfileService
{
    public OperationResult UpdateLoginProfilesAndDeviceRelations(List<LoginProfile> profiles, Guid deviceId)
    {
        var currentRelations = GetPhysicalDeviceLoginProfileRelationships(deviceId);
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
                AssignLoginProfileToPhysicalDevice(relationToAdd);
            }

            foreach (var relationToRemove in toRemove)
            {
                RemoveLoginProfileFromPhysicalDevice(relationToRemove.Id);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return new OperationResult(){IsSuccessful = false, Message = e.Message };
        }
        
        
        return new OperationResult();
    }

    public Guid AddLoginProfile(LoginProfile profile)
    {
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        database.LoginProfiles.Add(profile);
        database.SaveChanges();
        return id;
    }

    public Guid UpsertLoginProfile(LoginProfile profile)
    {
        if (database.LoginProfiles.Any(x => x.Id == profile.Id))
        {
            database.Attach(profile);
            database.LoginProfiles.Update(profile);
            database.SaveChanges();
            return profile.Id;
        }
        
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        database.LoginProfiles.Add(profile);
        database.SaveChanges();
        return id;
    }

    public List<LoginProfile> GetAllLoginProfiles()
    {
        return database.LoginProfiles.AsNoTracking().ToList();
    }

    public LoginProfile? GetLoginProfile(Guid id)
    {
        return database.LoginProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public List<LoginProfile> GetPhysicalDeviceLoginProfiles(Guid deviceId)
    {
        return database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Include(x => x.LoginProfile).Select(x => x.LoginProfile).ToList();
    }

    public List<LoginProfileToPhysicalDevice> GetPhysicalDeviceLoginProfileRelationships(Guid deviceId)
    {
        return database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Include(x => x.LoginProfile).ToList();
    }

    public Guid AssignLoginProfileToPhysicalDevice(LoginProfileToPhysicalDevice profile)
    {
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        database.LoginProfilesToPhysicalDevices.Add(profile);
        database.SaveChanges();
        return id;
    }

    public OperationResult RemoveLoginProfileFromPhysicalDevice(Guid relationId)
    {
        var item = database.LoginProfilesToPhysicalDevices.FirstOrDefault(x => x.Id == relationId);
        if (item != null)
        {
            database.LoginProfilesToPhysicalDevices.Remove(item);
            database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Login profile not found." };
    }

    public OperationResult RemoveLoginProfile(Guid id)
    {
        var profile = database.LoginProfiles.FirstOrDefault(x => x.Id == id);
        if (profile == null)
            return new OperationResult() { IsSuccessful = false, Message = "Login profile not found." };

        var relations = database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.LoginProfileId == id);
        database.LoginProfilesToPhysicalDevices.RemoveRange(relations);
        database.SaveChanges();

        database.LoginProfiles.Remove(profile);
        database.SaveChanges();
        return new OperationResult();
    }
}