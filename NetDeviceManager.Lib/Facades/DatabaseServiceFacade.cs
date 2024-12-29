using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class DatabaseServiceFacade(DatabaseService databaseService, ILogger<DatabaseService> logger) : IDatabaseService
{
    public Guid AddSchedulerJob(SchedulerJob job)
    {
        var result = databaseService.AddSchedulerJob(job);
        logger.LogInformation($"Added SchedulerJob with id: {result}");
        return result;
    }

    public Guid? UpsertCorrectDataPattern(CorrectDataPattern pattern)
    {
        var result = databaseService.UpsertCorrectDataPattern(pattern);
        logger.LogInformation($"Upserted CorrectDataPattern with id: {result}");
        return result;
    }

    public List<SchedulerJob> GetSchedulerJobs()
    {
        var result = databaseService.GetSchedulerJobs();
        logger.LogInformation($"Retrieved {result.Count} SchedulerJobs");
        return result;
    }

    public CorrectDataPattern? GetSpecificPattern(Guid deviceId, Guid sensorId)
    {
        var result = databaseService.GetSpecificPattern(deviceId, sensorId);
        logger.LogInformation($"Retrieved CorrectDataPattern with id: {result}");
        return result;
    }

    public OperationResult DeleteCorrectDataPattern(Guid id)
    {
        var result = databaseService.DeleteCorrectDataPattern(id);
        logger.LogInformation($"Deleted CorrectDataPattern with id: {result}");
        return result;
    }

    public OperationResult DeleteDeviceSchedulerJob(Guid id)
    {
        var result = databaseService.DeleteDeviceSchedulerJob(id);
        logger.LogInformation($"Deleted DeviceSchedulerJob with id: {result}");
        return result;
    }

    public OperationResult DeleteUser(string id)
    {
        var result = databaseService.DeleteUser(id);
        logger.LogInformation($"Deleted User with id: {result}");
        return result;
    }
}