using Quartz;
using Quartz.Spi;

namespace ScheduledDatabaseReporter.Factories;

public class DIJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DIJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return (IJob)_serviceProvider.GetService(bundle.JobDetail.JobType)!;
    }

    public void ReturnJob(IJob job)
    {
        var disposable = job as IDisposable;
        disposable?.Dispose();
    }
}