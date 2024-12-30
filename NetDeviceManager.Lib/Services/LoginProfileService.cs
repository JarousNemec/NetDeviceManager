using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Helpers;
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
                toAdd.Add(new LoginProfileToPhysicalDevice()
                    { LoginProfileId = profile.Id, PhysicalDeviceId = deviceId });
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
            return new OperationResult() { IsSuccessful = false, Message = e.Message };
        }


        return new OperationResult();
    }

    public Guid AddLoginProfile(LoginProfile profile)
    {
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        var obfuscated = ObfuscationHelper.ObfuscateLoginProfile(profile);
        database.LoginProfiles.Add(obfuscated);
        database.SaveChanges();
        return id;
    }

    public Guid UpsertLoginProfile(LoginProfile profile)
    {
        if (database.LoginProfiles.Any(x => x.Id == profile.Id))
        {
            var obfuscated = ObfuscationHelper.ObfuscateLoginProfile(profile);
            database.Attach(obfuscated);
            database.LoginProfiles.Update(obfuscated);
            database.SaveChanges();
            return obfuscated.Id;
        }

        return AddLoginProfile(profile);
    }

    public List<LoginProfile> GetAllLoginProfiles()
    {
        var profiles = database.LoginProfiles.AsNoTracking().ToList();
        return profiles.Select(ObfuscationHelper.DeobfuscateLoginProfile).ToList();
    }

    public LoginProfile? GetLoginProfile(Guid id)
    {
        var result = database.LoginProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
        return result != null ? ObfuscationHelper.DeobfuscateLoginProfile(result) : null;
    }

    public List<LoginProfile> GetPhysicalDeviceLoginProfiles(Guid deviceId)
    {
        return database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Include(x => x.LoginProfile).Select(x => x.LoginProfile).ToList();
    }

    public List<LoginProfileToPhysicalDevice> GetPhysicalDeviceLoginProfileRelationships(Guid deviceId)
    {
        var profiles = database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Include(x => x.LoginProfile).ToList();
        var deobfuscated = new List<LoginProfileToPhysicalDevice>();
        foreach (var profile in profiles)
        {
            if (profile.LoginProfile == null) continue;
            profile.LoginProfile = ObfuscationHelper.DeobfuscateLoginProfile(profile.LoginProfile);
            deobfuscated.Add(profile);
        }

        return deobfuscated;
    }

    public Guid AssignLoginProfileToPhysicalDevice(LoginProfileToPhysicalDevice profile)
    {
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        if (profile.LoginProfile != null)
            profile.LoginProfile = ObfuscationHelper.ObfuscateLoginProfile(profile.LoginProfile);
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