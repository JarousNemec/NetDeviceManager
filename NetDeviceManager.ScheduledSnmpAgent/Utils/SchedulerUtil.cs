using Lextm.SharpSnmpLib;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.ScheduledSnmpAgent.Jobs;
using Quartz;

namespace NetDeviceManager.ScheduledSnmpAgent.Utils;

public static class SchedulerUtil
{
    public static ITrigger CreateJobTrigger(string id, string cron, string groupId)
    {
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"T_{id}", groupId)
            .StartNow()
            .WithCronSchedule(cron)
            .Build();
        return trigger;
    }

    public static IJobDetail CreateReadDeviceSensorJob(string id, PhysicalDevice physicalDevice, List<SnmpSensorInPhysicalDevice> sensorsInPhysicalDevice, string groupId)
    {
        IJobDetail job = JobBuilder.Create<ReadDeviceSensorsJob>()
            .WithIdentity($"{id}", groupId).Build();
                    
        job.JobDataMap.Put("id", id);
        job.JobDataMap.Put("physicalDevice", physicalDevice);
        job.JobDataMap.Put("sensors", sensorsInPhysicalDevice);
        return job;
    }
    
}