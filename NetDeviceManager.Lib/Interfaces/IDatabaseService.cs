using NetDeviceManager.Database.Identity;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IDatabaseService
{
    Guid AddSchedulerJob(SchedulerJob job);

    Guid? UpsertCorrectDataPattern(CorrectDataPattern pattern);
    List<SchedulerJob> GetSchedulerJobs();

    CorrectDataPattern? GetSpecificPattern(Guid deviceId, Guid sensorId);
    OperationResult DeleteCorrectDataPattern(Guid id);

    OperationResult DeleteDeviceSchedulerJob(Guid id);

    OperationResult DeleteUser(string id);
}