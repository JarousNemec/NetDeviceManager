using Quartz;
using ScheduledDatabaseReporter.Jobs;

namespace ScheduledDatabaseReporter.Utils;

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
    public static IJobDetail CreateReportJob(string id, string path, string groupId)
    {
        IJobDetail job = JobBuilder.Create<ReporterJob>()
            .WithIdentity($"{id}", groupId).Build();

        job.JobDataMap.Put("id", id);
        job.JobDataMap.Put("path", path);
        
        
        return job;
    }
}