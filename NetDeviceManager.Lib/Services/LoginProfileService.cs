using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class LoginProfileService : ILoginProfileService
{
    private readonly IDatabaseService _databaseFacade;
    private readonly ApplicationDbContext _database;

    public LoginProfileService(ApplicationDbContext database, IDatabaseService databaseFacade)
    {
        _databaseFacade = databaseFacade;
        _database = database;
    }
    
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
        _database.LoginProfiles.Add(profile);
        _database.SaveChanges();
        return id;
    }

    public Guid UpsertLoginProfile(LoginProfile profile)
    {
        if (_database.LoginProfiles.Any(x => x.Id == profile.Id))
        {
            _database.Attach(profile);
            _database.LoginProfiles.Update(profile);
            _database.SaveChanges();
            return profile.Id;
        }
        
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        _database.LoginProfiles.Add(profile);
        _database.SaveChanges();
        return id;
    }

    public List<LoginProfile> GetLoginProfiles()
    {
        return _database.LoginProfiles.AsNoTracking().ToList();
    }

    public LoginProfile? GetLoginProfile(Guid id)
    {
        return _database.LoginProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public List<LoginProfile> GetPhysicalDeviceLoginProfiles(Guid deviceId)
    {
        return _database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Include(x => x.LoginProfile).Select(x => x.LoginProfile).ToList();
    }

    public List<LoginProfileToPhysicalDevice> GetPhysicalDeviceLoginProfileRelationships(Guid deviceId)
    {
        return _database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.PhysicalDeviceId == deviceId)
            .Include(x => x.LoginProfile).ToList();
    }

    public Guid AssignLoginProfileToPhysicalDevice(LoginProfileToPhysicalDevice profile)
    {
        var id = DatabaseUtil.GenerateId();
        profile.Id = id;
        _database.LoginProfilesToPhysicalDevices.Add(profile);
        _database.SaveChanges();
        return id;
    }

    public OperationResult RemoveLoginProfileFromPhysicalDevice(Guid relationId)
    {
        var item = _database.LoginProfilesToPhysicalDevices.FirstOrDefault(x => x.Id == relationId);
        if (item != null)
        {
            _database.LoginProfilesToPhysicalDevices.Remove(item);
            _database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Login profile not found." };
    }

    public OperationResult RemoveLoginProfile(Guid id)
    {
        var profile = _database.LoginProfiles.FirstOrDefault(x => x.Id == id);
        if (profile == null)
            return new OperationResult() { IsSuccessful = false, Message = "Login profile not found." };

        var relations = _database.LoginProfilesToPhysicalDevices.AsNoTracking().Where(x => x.LoginProfileId == id);
        _database.LoginProfilesToPhysicalDevices.RemoveRange(relations);
        _database.SaveChanges();

        _database.LoginProfiles.Remove(profile);
        _database.SaveChanges();
        return new OperationResult();
    }
}